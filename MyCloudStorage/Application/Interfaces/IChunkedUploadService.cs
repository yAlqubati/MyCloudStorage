using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCloudStorage.DTOs.File;

namespace MyCloudStorage.Application.Interfaces
{
    public interface IChunkedUploadService
    {
        Task<StartUploadResponseDto> StartUploadAsync(StartUploadRequestDto request, string ownerId);
        Task<ChunkUploadResponseDto> UploadChunkAsync(Guid sessionId, int chunkIndex, IFormFile chunk, string ownerId);
        Task CancelUploadAsync(Guid sessionId, string ownerId);
    }
}