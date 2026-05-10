using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCloudStorage.DTOs.File;

namespace MyCloudStorage.Application.Interfaces
{
    public interface IFileService
    {
        Task<FileResponseDto> CreateFile(CreateFileRequestDto request, string ownerId);
        Task<FileResponseDto> RenameFile(string ownerId, Guid fileId, RenameFileRequestDto request);
        Task<bool> DeleteFile(string ownerId, Guid fileId);
        Task<List<FileResponseDto>> GetFilesByFolder(string ownerId, Guid? folderId);
        Task<FileResponseDto?> GetFileById(Guid fileId, string ownerId);
    }
}