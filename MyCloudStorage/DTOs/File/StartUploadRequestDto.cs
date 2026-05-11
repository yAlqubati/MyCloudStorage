using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.DTOs.File
{
    public class StartUploadRequestDto
    {
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long TotalSize { get; set; }
        public int TotalChunks { get; set; }
        public Guid? FolderId { get; set; }

    }
}