using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.Domain.Entities
{
    public class Folder
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? ParentFolderId{get;set;}
        public string OwnerId {get; set;} = string.Empty;

        public User? Owner {get; set;}
        public Folder? ParentFolder {get; set;}

        public ICollection<Folder> ChildFolders { get; set; } = new List<Folder>();
        public ICollection<FileEntity> Files { get; set; } = new List<FileEntity>();

        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    }
}