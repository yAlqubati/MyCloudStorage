using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyCloudStorage.Application.Interfaces;
using MyCloudStorage.Data;
using MyCloudStorage.Domain.Entities;

namespace MyCloudStorage.Repositories
{
    public class FolderRepo : IFolderRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FolderRepo> _logger;

        public FolderRepo(ApplicationDbContext context, ILogger<FolderRepo> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Folder?> GetByIdAsync(Guid? id, string ownerId)
        {
            return await _context.Folders
                            .Include( f => f.ChildFolders)
                            .Include(f => f.Files)
                            .FirstOrDefaultAsync(f => f.Id == id && f.OwnerId == ownerId);
        }

        public async Task<List<Folder>> GetChildFolderAsync(Guid parentId, string ownerId)
        {
            return await _context.Folders
                        .Where(f => f.ParentFolderId == parentId && f.OwnerId == ownerId)
                        .OrderBy(f => f.Name)
                        .ToListAsync();
        }

        public async Task<List<Folder>> GetRootFoldersAsync(string ownerId)
        {
            return await _context.Folders
                            .Where(f => f.OwnerId == ownerId && f.ParentFolderId == null)
                            .ToListAsync();
        }

        public async Task<bool> ExistsAsync( Guid? FolderId, string ownerId)
        {
            return await _context.Folders
                .AnyAsync(f =>
                    f.Id == FolderId &&
                    f.OwnerId == ownerId);
        }

        public async Task<bool> ExistsAsync(string name, Guid? parentFolderId, string ownerId)
        {
            return await _context.Folders
                .AnyAsync(f =>
                    f.Name == name &&
                    f.ParentFolderId == parentFolderId &&
                    f.OwnerId == ownerId);
        }


        public async Task CreateFolderAsync(Folder folder)
        {
            await _context.Folders.AddAsync(folder);
        }

        public async Task DeleteFolder(Folder folder)
        {
            _context.Folders.Remove(folder);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


    }
}