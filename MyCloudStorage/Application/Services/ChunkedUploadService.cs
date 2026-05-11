using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Antiforgery;
using MyCloudStorage.Application.Interfaces;
using MyCloudStorage.Domain.Entities;
using MyCloudStorage.DTOs.File;

namespace MyCloudStorage.Application.Services
{
    public class ChunkedUploadService : IChunkedUploadService
    {
        private readonly IUploadSessionRepo _sessionRepo;
        private readonly IFileRepo _fileRepo;
        private readonly IFolderRepo _folderRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<ChunkedUploadService> _logger;
        private readonly string _basePath;
        private readonly string _tempPath;

        public ChunkedUploadService(
            IUploadSessionRepo sessionRepo,
            IFileRepo fileRepo,
            IFolderRepo folderRepo,
            IMapper mapper,
            IConfiguration config,
            ILogger<ChunkedUploadService> logger)
        {
            _sessionRepo = sessionRepo;
            _fileRepo = fileRepo;
            _folderRepo = folderRepo;
            _mapper = mapper;
            _logger = logger;
            _basePath = config["Storage:BasePath"] ?? "uploads";
            _tempPath = config["Storage:TempPath"] ?? "uploads/temp";
        }

        public async Task CancelUploadAsync(Guid sessionId, string ownerId)
        {
            var session = await _sessionRepo.GetSessionById(sessionId, ownerId);
            if(session is null) return;

            await CleanupSessionAsync(session);
            _logger.LogInformation("Upload session {SessionId} cancelled", sessionId);
        }

        public async Task<StartUploadResponseDto> StartUploadAsync(StartUploadRequestDto request, string ownerId)
        {
            if (request.FolderId.HasValue)
            {
                var folder = await _folderRepo.GetByIdAsync(request.FolderId.Value, ownerId);
                if(folder is null ) throw new InvalidOperationException("Target file doesn't exist");
            }

            _logger.LogInformation("folder id {folderid}", request.FolderId);
            if(await _fileRepo.ExistAsync(request.FileName, request.FolderId, ownerId))
            {
                throw new InvalidOperationException("A file with that name already exists here.");
            }

            if(request.TotalChunks <= 0)
            {
                throw new InvalidOperationException("TotalChunks must be at least 1.");
            }

            var sessionId = Guid.NewGuid();
            var tempDir = Path.Combine(_tempPath, sessionId.ToString());
            Directory.CreateDirectory(tempDir);

            var uploadSession = new UploadSession
            {
                Id = sessionId,
                FileName = request.FileName,
                FileType = request.FileType,
                FolderId = request.FolderId.Value,
                TotalSize = request.TotalSize,
                TotalChunks = request.TotalChunks,
                UserId = ownerId,
                TempDirectory = tempDir
            };

            await _sessionRepo.CreateUploadSession(uploadSession);
            await _sessionRepo.SaveChangesAsync();

            _logger.LogInformation("Upload session {SessionId} started for {FileName} ({Chunks} chunks)",
                sessionId, request.FileName, request.TotalChunks);

            return new StartUploadResponseDto
            {
                SessionId = sessionId,
                FileName = request.FileName,
                TotalChunks = request.TotalChunks,

            };
        }

        public async Task<ChunkUploadResponseDto> UploadChunkAsync(Guid sessionId, int chunkIndex, IFormFile chunk, string ownerId)
        {
            var session = await _sessionRepo.GetSessionById(sessionId, ownerId);

            if(session is null)
                throw new InvalidOperationException("Upload session not found.");

            if(session.ExpiresAt < DateTime.UtcNow)
            {
                await CleanupSessionAsync(session);
                throw new InvalidOperationException("Upload session has expired.");
            }

            if (chunkIndex < 0 || chunkIndex >= session.TotalChunks)
                throw new InvalidOperationException(
                    $"Invalid chunk index {chunkIndex}. Expected 0 to {session.TotalChunks - 1}.");

            var chunkPath = Path.Combine(session.TempDirectory, $"chunk_{chunkIndex}");
            if (File.Exists(chunkPath))
                throw new InvalidOperationException($"Chunk {chunkIndex} has already been uploaded.");

            using(var stream = new FileStream(chunkPath, FileMode.Create, FileAccess.Write))
            {
                await chunk.CopyToAsync(stream);
            }

            session.ReceivedChunks++;
            await _sessionRepo.SaveChangesAsync();

            _logger.LogInformation("Chunk {Index}/{Total} received for session {SessionId}",
                chunkIndex + 1, session.TotalChunks, sessionId);

            if(session.ReceivedChunks == session.TotalChunks)
            {
                var fileDto = await AssembleFileAsync(session);

                return new ChunkUploadResponseDto
                {
                    SessionId = sessionId,
                    ReceivedChunks = session.ReceivedChunks,
                    TotalChunks = session.TotalChunks,
                    IsComplete = true,
                    File = fileDto
                };
            }

            return new ChunkUploadResponseDto
            {
                SessionId = sessionId,
                ReceivedChunks = session.ReceivedChunks,
                TotalChunks = session.TotalChunks,
                IsComplete = false
            };
        }

        public async Task<FileResponseDto> AssembleFileAsync(UploadSession session)
        {
            var extension = Path.GetExtension(session.FileName);
            var storageKey = $"{session.UserId}/{Guid.NewGuid()}{extension}";
            var finalPath = Path.Combine(_basePath, storageKey);

            Directory.CreateDirectory(Path.GetDirectoryName(finalPath)!);

            using(var finalStream = new FileStream(finalPath, FileMode.Create, FileAccess.Write))
            {
                for(int i = 0 ; i < session.TotalChunks; i++)
                {
                    var chunkPath = Path.Combine(session.TempDirectory, $"chunk_{i}");

                    if (!File.Exists(chunkPath))
                        throw new InvalidOperationException(
                            $"Chunk {i} is missing. Cannot assemble file.");

                    using var chunkStream = new FileStream(chunkPath, FileMode.Open, FileAccess.Read);
                    await chunkStream.CopyToAsync(finalStream);
                }
            }

            var fileMetaData = new FileEntity
            {
                Name = session.FileName,
                Size = session.TotalSize,
                FileType = session.FileType,
                StorageKey = storageKey,
                FolderId = session.FolderId,
                UserId = session.UserId
            };

            await _fileRepo.CreateFileAsync(fileMetaData);
            await _fileRepo.SaveChangesAsync();

            await CleanupSessionAsync(session);
            await _sessionRepo.SaveChangesAsync();

            _logger.LogInformation("File {FileName} assembled from {Chunks} chunks, StorageKey: {Key}",
                session.FileName, session.TotalChunks, storageKey);

            return _mapper.Map<FileResponseDto>(fileMetaData);
        }

        private async Task CleanupSessionAsync(UploadSession session)
        {
            if (Directory.Exists(session.TempDirectory))
                Directory.Delete(session.TempDirectory, recursive: true);

            await _sessionRepo.DeleteSession(session);
            await _sessionRepo.SaveChangesAsync();
        }


    }
}