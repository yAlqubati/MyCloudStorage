using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.DTOs.File
{
    public class FileResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public Guid FolderId { get; set; }
        public DateTime CreatedAt { get; set; }
        
    }
}