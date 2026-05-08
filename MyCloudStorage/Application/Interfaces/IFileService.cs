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
        Task<FileResponseDto> RenameFile(string ownerId, int fileId, RenameFileRequestDto request);
        Task<bool> DeleteFile(string ownerId, int fileId);
        Task<List<FileResponseDto>> GetFilesByFolder(string ownerId, int? folderId);
        Task<FileResponseDto?> GetFileById(int fileId, string ownerId);
    }
}