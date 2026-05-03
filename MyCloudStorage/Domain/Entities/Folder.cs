using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.Domain.Entities
{
    public class Folder
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ParentFolderId{get;set;}
        public string OwnerId {get; set;} = string.Empty;

        public User Owner {get; set;}
    }
}