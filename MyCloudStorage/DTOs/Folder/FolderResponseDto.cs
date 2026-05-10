using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.DTOs.Folder
{
    public class FolderResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid ParentFolderId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}