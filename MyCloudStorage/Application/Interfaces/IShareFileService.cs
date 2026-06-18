using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.Application.Interfaces
{
    public interface IShareFileService
    {
        Task<FileShareResponseDto> ShareFile(CreateFileShareRequestDto request);
        Task<List<FileShareResponseDto>> GetFilesSharedWithMe(string userId);
        Task<List<FileShareResponseDto>> GetSharesForFile(Guid fileId, string ownerId);
        Task RevokeFileShare(Guid shareId, string ownerId);
    }
}