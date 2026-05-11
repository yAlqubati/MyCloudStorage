using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.DTOs.File
{
    public class StartUploadResponseDto
    {
        public Guid SessionId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public int TotalChunks { get; set; }
    }
}