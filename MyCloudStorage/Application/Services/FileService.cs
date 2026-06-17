using AutoMapper;
using MyCloudStorage.Application.Interfaces;
using MyCloudStorage.Domain.Entities;
using MyCloudStorage.DTOs.File;
using MyCloudStorage.Exceptions;

namespace MyCloudStorage.Application.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepo _fileRepo;
        private readonly IMapper _mapper;
        private readonly IFolderRepo _folderRepo;
        private readonly ILogger<FileService> _logger;
        private readonly IStorageService _storageService;

        public FileService(IFileRepo fileRepo, IMapper mapper, IFolderRepo folderRepo, ILogger<FileService> logger, IStorageService storageService)
        {
            _fileRepo = fileRepo;
            _mapper = mapper;
            _folderRepo = folderRepo;
            _logger = logger;
            _storageService = storageService;
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

        public async Task<FileResponseDto?> GetFileById(Guid fileId, string ownerId)
        {
            var file = await _fileRepo.GetByIdAsync(fileId, ownerId);

            return _mapper.Map<FileResponseDto>(file);
        }

        public async Task<List<FileResponseDto>> GetFilesByFolder(string ownerId, Guid? folderId)
        {
            if (folderId.HasValue)
            {
                var folder = await _folderRepo.GetByIdAsync(folderId.Value, ownerId);
                if (folder is null)
                    throw new InvalidOperationException("This folder does not exist.");
            }

            var files = await _fileRepo.GetFilesByFolderAsync(folderId, ownerId);
            return _mapper.Map<List<FileResponseDto>>(files);
        }

        public async Task<FileResponseDto> RenameFile(string ownerId, Guid fileId, RenameFileRequestDto request)
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

        public async Task<bool> DeleteFile(string ownerId, Guid fileId)
        {
            var file = await _fileRepo.GetByIdAsync(fileId, ownerId);
            if (file is null) return false;

            await _fileRepo.DeleteFile(file);
            await _fileRepo.SaveChangesAsync();

            return true;
        }

        public async Task<(Stream stream, string contentType, string fileName)> DownloadFileAsync(Guid fileId, string ownerId)
        {
            var file = await _fileRepo.GetByIdAsync(fileId,ownerId);
            if(file is null)    throw new InvalidOperationException("File not found.");

            var stream = await _storageService.GetFileAsync(file.StorageKey);

        _logger.LogInformation("User {UserId} downloading file {FileId}", ownerId, fileId);

        return (stream, file.FileType, file.Name);

        }

        public async Task<FileResponseDto> MoveFile(MoveFileRequestDto request, string ownerId)
        {
            var file = await _fileRepo.GetByIdAsync(request.Id, ownerId);
            if(file is null)    throw new InvalidOperationException("File Not found");

            if(file.FolderId == request.DestinationFolderId)    throw new InvalidOperationException("File is already in that folder.");

            var sourceFolder = await _folderRepo.ExistsAsync(request.SourceFolderId, ownerId);
            if(!sourceFolder) throw new InvalidOperationException("Source folder Not found");

            var destinationFolder = await _folderRepo.ExistsAsync(request.DestinationFolderId, ownerId);
            if(!destinationFolder) throw new InvalidOperationException("Destination folder Not found");


            var nameExists = await _fileRepo.ExistAsync(file.Name, request.DestinationFolderId, ownerId);
            if (nameExists) throw new ConflictException("A file with that name already exists in the destination folder.");

            file.FolderId = request.DestinationFolderId;
            await _fileRepo.SaveChangesAsync();

            return _mapper.Map<FileResponseDto>(file);

        }
    }
}