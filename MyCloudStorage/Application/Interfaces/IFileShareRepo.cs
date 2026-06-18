using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCloudStorage.Domain.Entities;

namespace MyCloudStorage.Application.Interfaces
{
    public interface IFileShareRepo
    {
        Task AddShareFileAsync(FileShareEntity sharedFile);
        Task<FileShareEntity?> GetFileShareAsync(Guid shareId, string ownerId);
        Task<List<FileShareEntity>> GetSharesForFileAsync(Guid fileId, string ownerId);
        Task <List<FileShareEntity>> GetFilesSharedWithMeAsync(string userId);
        Task<bool> AlreadySharedAsync(Guid fileId, string sharedWithId);
        Task DeleteFileShareAsync(FileShareEntity share);

        Task SaveChangesAsync();

    }
}