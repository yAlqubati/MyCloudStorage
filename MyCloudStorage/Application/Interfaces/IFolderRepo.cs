using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCloudStorage.Domain.Entities;

namespace MyCloudStorage.Application.Interfaces
{
    public interface IFolderRepo
    {
        Task CreateFolderAsync(Folder folder);
        Task DeleteFolder(Folder folder);
        Task<Folder?> GetByIdAsync(Guid? id, string ownerId);
        Task<List<Folder>> GetChildFolderAsync (Guid parentId, string ownerId);
        Task<List<Folder>> GetRootFoldersAsync(string ownerId);
        Task<bool> ExistsAsync(Guid? parentFolderId, string ownerId);

        Task SaveChangesAsync();


    }
}