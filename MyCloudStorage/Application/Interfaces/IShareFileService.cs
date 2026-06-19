using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCloudStorage.DTOs.File;

namespace MyCloudStorage.Application.Interfaces
{
    public interface IShareFileService
    {
        Task<FileShareResponseDto> ShareFile(CreateFileShareRequestDto request, string ownerId);
        Task<List<FileShareResponseDto>> GetFilesSharedWithMe(string userId);
        Task<List<FileShareResponseDto>> GetSharesForFile(Guid fileId, string ownerId);
        Task RevokeFileShare(Guid shareId, string ownerId);

        Task<(Stream stream, string contentType, string fileName)> DownloadViaShareAsync(string userId, Guid fileId);
    }
}