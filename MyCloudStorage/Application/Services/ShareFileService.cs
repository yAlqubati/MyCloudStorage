using MyCloudStorage.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using MyCloudStorage.Application.Interfaces;
using MyCloudStorage.DTOs.File;
using MyCloudStorage.Repositories;
using MyCloudStorage.Exceptions;
using MyCloudStorage.Domain.Enums;

namespace MyCloudStorage.Application.Services
{
    public class ShareFileService : IShareFileService
    {

        private readonly IFileShareRepo _shareRepo;
        private readonly IFileRepo _fileRepo;
        private readonly IStorageService _storageService;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ShareFileService> _logger;


        public ShareFileService(
            IFileShareRepo shareRepo,
            IFileRepo fileRepo,
            IStorageService storageService,
            UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ShareFileService> logger
            )
        {
            _shareRepo = shareRepo;
            _fileRepo = fileRepo;
            _storageService = storageService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<(Stream stream, string contentType, string fileName)> DownloadViaShareAsync(string userId, Guid fileId)
        {
            var shares = await _shareRepo.GetFilesSharedWithMeAsync(userId);
            
            var share = shares.FirstOrDefault(s =>
                s.FileId == fileId &&
                s.Permission >= SharePermission.Download &&
                (s.ExpiresAt == null || s.ExpiresAt > DateTime.UtcNow));

            if (share is null)
                throw new ForbiddenException("You do not have download access to this file.");

            var stream = await _storageService.GetFileAsync(share.File.StorageKey);
            return (stream, share.File.FileType, share.File.Name);

        }

        public async Task<List<FileShareResponseDto>> GetFilesSharedWithMe(string userId)
        {
            var shares = await _shareRepo.GetFilesSharedWithMeAsync(userId);

            return shares.Select(s => new FileShareResponseDto
            {
                Id = s.Id,
                FileId = s.FileId,
                FileName = s.File.Name,
                SharedWithEmail = s.Owner.Email ?? "",
                Permission = s.Permission.ToString(),
                CreatedAt = s.CreatedAt,
                ExpiresAt = s.ExpiresAt
            }).ToList();
        }

        public async Task<List<FileShareResponseDto>> GetSharesForFile(Guid fileId, string ownerId)
        {
            var targetFile = await _fileRepo.GetByIdAsync(fileId, ownerId);
            if(targetFile is null)
                throw new NotFoundException("File not found");
            // CHECK THIS LATER, WHY ARE YOU MAKING TWO QUERIES TO THE DB?
            var shares = await _shareRepo.GetSharesForFileAsync(fileId, ownerId);

            _logger.LogInformation("shares: ${@shares}",shares);
            return shares.Select(s => new FileShareResponseDto
            {
                Id = s.Id,
                FileId = s.FileId,
                FileName = targetFile.Name,
                SharedWithEmail = s.SharedWith.Email ?? "",
                Permission = s.Permission.ToString(),
                CreatedAt = s.CreatedAt,
                ExpiresAt = s.ExpiresAt
            }).ToList();
        }

        public async Task RevokeFileShare(Guid shareId, string ownerId)
        {
            var share = await _shareRepo.GetFileShareAsync(shareId, ownerId);
            if(share is null)
                throw new NotFoundException("Share not found");

            await _shareRepo.DeleteFileShareAsync(share);
            await _shareRepo.SaveChangesAsync();

            _logger.LogInformation("User {OwnerId} revoked share {ShareId}", ownerId, shareId);
        }

        public async Task<FileShareResponseDto> ShareFile(CreateFileShareRequestDto request, string ownerId)
        {

            
            var targetUser = await _userManager.FindByEmailAsync(request.SharedWithEmail);
            if (targetUser is null)
                throw new NotFoundException($"No user found with email '{request.SharedWithEmail}'.");

            var targetFile = await _fileRepo.GetByIdAsync(request.FileId, ownerId);
            if(targetFile is null)
                throw new NotFoundException("File not found");
                

            if (targetUser.Id == ownerId)
                throw new ValidationException("You cannot share a file with yourself.");

            // !!!!!!!
            // THIS NEEDS UPDATE LATER, WHAT IF THERE EXIST A FILE SHARE BUT IT IS EXPERIED
            var alreadyShared = await _shareRepo.AlreadySharedAsync(request.FileId, ownerId);
            if(alreadyShared)
                throw new ConflictException("the file is already shared with the selected user");

            var share = new FileShareEntity
            {
                Id = Guid.NewGuid(),
                FileId = request.FileId,
                OwnerId = ownerId,
                SharedWithId = targetUser.Id,
                Permission = request.Permission,
                ExpiresAt = request.ExpiresAt
            };

            await _shareRepo.AddShareFileAsync(share);
            await _shareRepo.SaveChangesAsync();

            return new FileShareResponseDto
            {
                Id = share.Id,
                FileId = targetFile.Id,
                FileName = targetFile.Name,
                SharedWithEmail = request.SharedWithEmail,
                Permission = share.Permission.ToString(),
                CreatedAt = share.CreatedAt,
                ExpiresAt = share.ExpiresAt
            };

        }
    }
}