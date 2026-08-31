using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MyCloudStorage.Domain.Entities
{
    public class User : IdentityUser
    {
        public long StorageUsed { get; set; } = 0;
        public long StorageQuota { get; set; } = 3L * 1024 * 1024 * 1024;
    }
}