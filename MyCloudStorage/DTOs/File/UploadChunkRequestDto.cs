namespace MyCloudStorage.DTOs.File
{
    public class UploadChunkRequestDto
    {
        public Guid SessionId { get; set; }
        public int ChunkIndex { get; set; }
        public IFormFile Chunk { get; set; } = null!;
    }
}