using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MyCloudStorage.Application.Interfaces;
using MyCloudStorage.Domain.Entities;
using MyCloudStorage.DTOs.Folder;

namespace MyCloudStorage.Application.Services
{
    public class FolderService : IFolderService
    {
        private readonly IFolderRepo _folderRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<FolderService> _logger;

        public FolderService(IFolderRepo folderRepo, IMapper mapper, ILogger<FolderService> logger)
        {
            _folderRepo = folderRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<FolderResponseDto> CreateFolder(CreateFolderRequestDto request, string ownerId)
        {
            var exist = await _folderRepo.ExistsAsync(request.Name,request.ParentFolderId, ownerId);
            if (exist)
            {
                throw new InvalidOperationException("A folder with that name already exists here.");
            }

            var folder = _mapper.Map<Folder>(request);
            folder.OwnerId = ownerId;


            await _folderRepo.CreateFolderAsync(folder);
            await _folderRepo.SaveChangesAsync();

            return _mapper.Map<FolderResponseDto>(folder);
        }

        public async Task<bool> DeleteFolder(string ownerId, Guid folderId)
        {
            var wantedFolder = await _folderRepo.GetByIdAsync(folderId, ownerId);
            if(wantedFolder is null) return false;

            await _folderRepo.DeleteFolder(wantedFolder);
            await _folderRepo.SaveChangesAsync();

            return true;
        }

        public async Task<List<FolderResponseDto>> GetChildFolders(string ownerId, Guid parentId)
        {
            var exist = await _folderRepo.GetByIdAsync(parentId, ownerId);
            if(exist is null)   throw new InvalidOperationException("This folder does not exist.");

            var folder = await _folderRepo.GetChildFolderAsync(parentId, ownerId);
            
            return _mapper.Map<List<FolderResponseDto>>(folder);
            
        }

        public async Task<FolderResponseDto> GetFolderById(Guid folderId, string ownerId)
        {
            var exist = await _folderRepo.GetByIdAsync(folderId, ownerId);
            if(exist is null)   throw new InvalidOperationException("This folder does not exist.");

            return _mapper.Map<FolderResponseDto>(exist);
        }

        public async Task<List<FolderResponseDto>> GetRootFolders(string ownerId)
        {
            var folders = await _folderRepo.GetRootFoldersAsync(ownerId);

            return _mapper.Map<List<FolderResponseDto>>(folders);
        }

        public async Task<FolderResponseDto> RenameFolder(string ownerId, Guid folderId, RenameFolderRequestDto request)
        {
            var exist = await _folderRepo.GetByIdAsync(folderId, ownerId);
            if(exist is null ) throw new InvalidOperationException("This folder does not exist.");

            exist.Name = request.NewName;
            await _folderRepo.SaveChangesAsync();

            return _mapper.Map<FolderResponseDto>(exist);
        }
    }
}