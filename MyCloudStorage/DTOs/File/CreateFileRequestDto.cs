using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.DTOs.File
{
    public class CreateFileRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public string FileType { get; set; } = string.Empty;
        public string StorageKey { get; set; } = string.Empty;

        private int? _folderId;
        public int? FolderId
        {
            get => _folderId;
            set => _folderId = (value == 0) ? null : value;  // treat 0 as root
        }
    }
}