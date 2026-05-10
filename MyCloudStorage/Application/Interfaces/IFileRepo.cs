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
        Task<FileEntity?> GetByIdAsync(Guid fileId, string ownerId);
        Task<List<FileEntity>> GetFilesByFolderAsync(Guid? folderId, string ownerId);
        Task<bool> ExistAsync(string name,Guid? folderId, string ownerId);
        Task SaveChangesAsync();
    }
}