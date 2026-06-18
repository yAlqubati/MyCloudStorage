using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.DTOs.File
{
    public class FileShareResponseDto
    {
        public Guid Id { get; set; }
        public Guid FileId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string SharedWithEmail { get; set; } = string.Empty;
        public string Permission { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}