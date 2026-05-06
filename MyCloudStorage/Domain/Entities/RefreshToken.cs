using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.Domain.Entities
{
    public class RefreshToken
    {
        public int Id {get; set;}

        public string HashToken { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;

        public DateTime ExpireAt { get; set; }
        public bool IsExpired { get; set; } = false;

        public bool IsRevoked { get; set; } = false;
        public DateTime RevokedAt { get; set; }

        public string? ReplacingToken { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User? User {get;set;}
    }
}