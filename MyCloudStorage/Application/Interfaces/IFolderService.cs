using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCloudStorage.DTOs.Folder;

namespace MyCloudStorage.Application.Interfaces
{
    public interface IFolderService
    {
        public Task<FolderResponseDto> CreateFolder(CreateFolderRequestDto request, string ownerId);
        public Task<FolderResponseDto> RenameFolder(string ownerId, Guid folderId, RenameFolderRequestDto request);
        public Task<bool> DeleteFolder(string ownerId, Guid folderId);
        public Task<List<FolderResponseDto>> GetChildFolders(string ownerId, Guid parentId);
        public Task<List<FolderResponseDto>> GetRootFolders(string ownerId);
        public Task<FolderResponseDto> GetFolderById(Guid folderId, string ownerId);

    }
}