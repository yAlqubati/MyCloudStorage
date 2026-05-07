using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.Domain.Entities
{
    public class FileEntity
    {
        public int Id {get;set;}
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public string StorageKey { get; set; } = string.Empty;
        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
        public string FileType {get;set;} = string.Empty;
        
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; }


        public int FolderId {get; set;}
        public Folder Folder {get; set;}
        
    }
}