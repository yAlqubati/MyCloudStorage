using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.Domain.Entities
{
    public class UploadSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long TotalSize { get; set; }
        public int TotalChunks { get; set; }
        public int ReceivedChunks { get; set; } = 0;
        public Guid? FolderId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string TempDirectory { get; set; } = string.Empty;
        public UploadStatus Status {get;set;} = UploadStatus.Active;
        public string StatusMessage {get;set;} = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(24);
    }
}