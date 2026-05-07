using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.DTOs.Folder
{
    public class CreateFolderRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public int ParentFolderId { get; set; }
    }
}