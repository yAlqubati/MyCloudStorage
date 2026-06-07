using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.DTOs.File
{
    public class MoveFileRequestDto
    {
        public Guid Id {get;set;}
        public Guid SourceFolderId {get;set;}
        public Guid DestinationFolderId {get;set;}

    }
}