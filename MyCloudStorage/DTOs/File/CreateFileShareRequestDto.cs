using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCloudStorage.Domain.Enums;

namespace MyCloudStorage.DTOs.File
{
    public class CreateFileShareRequestDto
    {
        public Guid FileId { get; set; }
        public string ShareWithEmail {get;set;} = string.Empty;
        public SharePermission Permission { get; set; } = SharePermission.Download;
        public DateTime? ExpiresAt { get; set; }
    }
}