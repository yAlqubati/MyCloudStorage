using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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

        public Task CancelUploadAsync(Guid sessionId, string ownerId)
        {
            throw new NotImplementedException();
        }

        public async Task<StartUploadResponseDto> StartUploadAsync(StartUploadRequestDto request, string ownerId)
        {
            if (request.FolderId.HasValue)
            {
                var folder = await _folderRepo.GetByIdAsync(request.FolderId.Value, ownerId);
                if(folder is null ) throw new InvalidOperationException("Target file doesn't exist");
            }

            if(_fileRepo.ExistAsync(request.FileName, request.FolderId.Value, ownerId) != null)
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

        public Task<ChunkUploadResponseDto> UploadChunkAsync(Guid sessionId, int chunkIndex, IFormFile chunk, string ownerId)
        {
            throw new NotImplementedException();
        }
    }
}