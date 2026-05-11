using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCloudStorage.Domain.Entities;

namespace MyCloudStorage.Application.Interfaces
{
    public interface IUploadSessionRepo
    {
        Task CreateUploadSession(UploadSession newSession);
        Task<UploadSession?> GetSessionById(Guid sessionId, string ownerId);
        Task DeleteSession(UploadSession session);
        Task SaveChangesAsync();
    }
}