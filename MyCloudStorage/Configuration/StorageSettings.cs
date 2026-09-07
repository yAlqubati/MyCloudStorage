using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.Configuration
{
    public class StorageSettings
    {
        public const string SectionName = "Storage";

        public string BasePath { get; set; } = "uploads";
        public string TempPath { get; set; } = "uploads/temp";


        // 3GB default quota per user
        public long DefaultUserQuotaBytes { get; set; } = 3L * 1024 * 1024 * 1024;

        public List<string> AllowedExtensions { get; set; } = new()
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp",
            ".pdf", ".txt", ".csv",
            ".doc", ".docx", ".xls", ".xlsx",
            ".zip", ".mp4", ".webm", ".mp3", ".wav"
        };

        public List<string> AllowedMimeTypes { get; set; } = new()
        {
            "image/jpeg", "image/png", "image/gif", "image/webp",
            "application/pdf", "text/plain", "text/csv",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.ms-excel",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/zip", "video/mp4", "video/webm",
            "audio/mpeg", "audio/wav"
        };

        public long DefaultUserQuotaGB => DefaultUserQuotaBytes / 1024 / 1024 / 1024;
    }
}