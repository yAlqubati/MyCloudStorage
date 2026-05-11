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
    public class UploadSessionRepo : IUploadSessionRepo
    {
        private readonly ApplicationDbContext _context;

        public UploadSessionRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateUploadSession(UploadSession newSession)
        {
            await _context.UploadSessions.AddAsync(newSession);
        }

        public async Task DeleteSession(UploadSession session)
        {
            _context.UploadSessions.Remove(session);
            await Task.CompletedTask;
        }

        public async Task<UploadSession?> GetSessionById(Guid sessionId, string ownerId)
        {
            return await _context.UploadSessions.FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == ownerId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}