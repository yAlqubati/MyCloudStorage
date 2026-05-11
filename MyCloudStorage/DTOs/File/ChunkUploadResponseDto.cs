using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.DTOs.File
{
    public class ChunkUploadResponseDto
    {
        public Guid SessionId { get; set; }
        public int ReceivedChunks { get; set; }
        public int TotalChunks { get; set; }
        public bool IsComplete { get; set; }
        public FileResponseDto? File { get; set; }
    }
}