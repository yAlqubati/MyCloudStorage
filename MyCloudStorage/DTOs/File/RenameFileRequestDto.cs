using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.DTOs.File
{
    public class RenameFileRequestDto
    {
        public string NewName { get; set; } = string.Empty;
    }
}