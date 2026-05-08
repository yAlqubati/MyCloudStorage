using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MyCloudStorage.Application.Interfaces;
using MyCloudStorage.Domain.Entities;
using MyCloudStorage.DTOs.File;

namespace MyCloudStorage.Application.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepo _fileRepo;
        private readonly IMapper _mapper;
        private readonly IFolderRepo _folderRepo;
        private readonly ILogger<FileService> _logger;

        public FileService(IFileRepo fileRepo, IMapper mapper, IFolderRepo folderRepo, ILogger<FileService> logger)
        {
            _fileRepo = fileRepo;
            _mapper = mapper;
            _folderRepo = folderRepo;
            _logger = logger;
        }

        public async Task<FileResponseDto> CreateFile(CreateFileRequestDto request, string ownerId)
        {
            if (request.FolderId.HasValue)
            {
                var folder = await _folderRepo.GetByIdAsync(request.FolderId, ownerId);
                if(folder is null)
                    throw new InvalidOperationException("The target folder does not exist.");
            }

            var exists = await _fileRepo.ExistAsync(request.Name, request.FolderId, ownerId);
            if (exists)
                throw new InvalidOperationException("A file with that name already exists here.");

            var file = _mapper.Map<FileEntity>(request);
            file.UserId = ownerId;

            await _fileRepo.CreateFileAsync(file);
            await _fileRepo.SaveChangesAsync();

            return _mapper.Map<FileResponseDto>(file);
        }

        public async Task<FileResponseDto?> GetFileById(int fileId, string ownerId)
        {
            var file = await _fileRepo.GetByIdAsync(fileId, ownerId);

            return _mapper.Map<FileResponseDto>(file);
        }

        public async Task<List<FileResponseDto>> GetFilesByFolder(string ownerId, int? folderId)
        {
            // If folderId provided, verify the folder exists and belongs to this user
            if (folderId.HasValue)
            {
                var folder = await _folderRepo.GetByIdAsync(folderId.Value, ownerId);
                if (folder is null)
                    throw new InvalidOperationException("This folder does not exist.");
            }

            var files = await _fileRepo.GetFilesByFolderAsync(folderId, ownerId);
            return _mapper.Map<List<FileResponseDto>>(files);
        }

        public async Task<FileResponseDto> RenameFile(string ownerId, int fileId, RenameFileRequestDto request)
        {
            var file = await _fileRepo.GetByIdAsync(fileId, ownerId);
            if (file is null)
                throw new InvalidOperationException("File not found.");

            var exists = await _fileRepo.ExistAsync(request.NewName, file.FolderId, ownerId);
            if (exists)
                throw new InvalidOperationException("A file with that name already exists here.");

            file.Name = request.NewName;
            await _fileRepo.SaveChangesAsync();

            return _mapper.Map<FileResponseDto>(file);
        }

        public async Task<bool> DeleteFile(string ownerId, int fileId)
        {
            var file = await _fileRepo.GetByIdAsync(fileId, ownerId);
            if (file is null) return false;

            await _fileRepo.DeleteFile(file);
            await _fileRepo.SaveChangesAsync();

            return true;
        }



    }
}