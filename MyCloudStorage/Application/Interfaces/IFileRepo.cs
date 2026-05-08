using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCloudStorage.Domain.Entities;

namespace MyCloudStorage.Application.Interfaces
{
    public interface IFileRepo
    {
        Task CreateFileAsync(FileEntity file);
        Task DeleteFile(FileEntity file);
        Task<FileEntity?> GetByIdAsync(int fileId, string ownerId);
        Task<List<FileEntity>> GetFilesByFolderAsync(int? folderId, string ownerId);
        Task<bool> ExistAsync(string name,int? folderId, string ownerId);
        Task SaveChangesAsync();
    }
}