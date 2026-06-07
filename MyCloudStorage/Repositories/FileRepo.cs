using Microsoft.EntityFrameworkCore;
using MyCloudStorage.Application.Interfaces;
using MyCloudStorage.Data;
using MyCloudStorage.Domain.Entities;

namespace MyCloudStorage.Repositories
{
    public class FileRepo : IFileRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FileRepo> _logger;

        public FileRepo(ApplicationDbContext context, ILogger<FileRepo> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CreateFileAsync(FileEntity newfile)
        {
            await _context.Files.AddAsync(newfile);
        }

        public async Task DeleteFile(FileEntity file)
        {
            _context.Files.Remove(file);
            await Task.CompletedTask;
        }

        public async Task<bool> ExistAsync(string name,Guid? folderId, string ownerId)
        {
             return await _context.Files
                    .AnyAsync(f => f.Name == name && f.UserId == ownerId && f.FolderId == folderId);
        }

        public async Task<FileEntity?> GetByIdAsync(Guid fileId, string ownerId)
        {
            return await _context.Files
                    .Include(f => f.Folder)
                    .FirstOrDefaultAsync(f => f.Id == fileId && f.UserId == ownerId);
        }

        public async Task<List<FileEntity>> GetFilesByFolderAsync(Guid? folderId, string ownerId)
        {
            return await _context.Files
                .Where(f => f.FolderId == folderId && f.UserId == ownerId)
                .OrderBy(f => f.Name)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}