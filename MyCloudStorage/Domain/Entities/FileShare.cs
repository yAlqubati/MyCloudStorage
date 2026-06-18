using MyCloudStorage.Domain.Enums;

namespace MyCloudStorage.Domain.Entities
{
    public class FileShareEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid FileId { get; set; }
        public FileEntity File { get; set; } = null!;

        public string OwnerId { get; set; } = string.Empty;
        public User Owner { get; set; } = null!;

        public string SharedWithId { get; set; } = string.Empty;
        public User SharedWith { get; set; } = null!;

        public SharePermission Permission { get; set; } = SharePermission.Download;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
    }
}