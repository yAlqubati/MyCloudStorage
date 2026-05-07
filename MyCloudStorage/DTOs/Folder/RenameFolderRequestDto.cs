using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.DTOs.Folder
{
    public class RenameFolderRequestDto
    {
        public string NewName { get; set; } = string.Empty;
    }
}