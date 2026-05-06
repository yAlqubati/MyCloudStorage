using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyCloudStorage.Domain.Entities;

namespace MyCloudStorage.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<FileEntity> Files {get; set;}
        public DbSet<Folder> Folders { get; set; }
        public DbSet<RefreshToken> RefreshTokens {get; set;}
    }
}