using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyCloudStorage.Application.Interfaces;
using MyCloudStorage.Data;
using MyCloudStorage.Domain.Entities;

namespace MyCloudStorage.Repositories
{
    public class FileShareRepo : IFileShareRepo
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<IFileShareRepo> _logger;

        public FileShareRepo(ApplicationDbContext context, ILogger<IFileShareRepo> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task AddShareFileAsync(FileShareEntity sharedFile)
        {
            await _context.SharedFiles.AddAsync(sharedFile);
        }

        public async Task<bool> AlreadySharedAsync(Guid fileId, string sharedWithId)
        {
            return await _context.SharedFiles.AnyAsync(s => s.FileId == fileId && s.SharedWithId == sharedWithId);
        }

        public async Task DeleteFileShareAsync(FileShareEntity share)
        {
            _context.SharedFiles.Remove(share);
            await Task.CompletedTask;
        }

        public async Task<FileShareEntity?> GetFileShareAsync(Guid shareId, string ownerId)
        {
            return await _context.SharedFiles
                        .Include(s => s.File)
                        .FirstOrDefaultAsync(s => s.Id == shareId && s.OwnerId == ownerId);
        }

        public async Task<List<FileShareEntity>> GetFilesSharedWithMeAsync(string userId)
        {
            return await _context.SharedFiles
                            .Include(s => s.File)
                            .Include(s => s.Owner)
                            .Where(s => s.SharedWithId == userId && ((s.ExpiresAt == null || s.ExpiresAt > DateTime.UtcNow)))
                            .ToListAsync();
        }

        public async Task<List<FileShareEntity>> GetSharesForFileAsync(Guid fileId, string ownerId)
        {
            return await _context.SharedFiles
                            .Include(s => s.SharedWith)
                            .Where(s => s.FileId == fileId && s.OwnerId == ownerId).ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}