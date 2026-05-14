using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCloudStorage.Application.Interfaces;
using MyCloudStorage.DTOs.File;

namespace MyCloudStorage.Controllers
{
    [ApiController]
    [Route("api/upload")]
    [Authorize]
    public class ChunkedUploadController : ControllerBase
    {
        private readonly IChunkedUploadService _uploadService;
        private readonly ILogger<ChunkedUploadController> _logger;
        private string OwnerId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public ChunkedUploadController(
            IChunkedUploadService uploadService,
            ILogger<ChunkedUploadController> logger)
        {
            _uploadService = uploadService;
            _logger = logger;
        }


        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] StartUploadRequestDto request)
        {
            var result = await _uploadService.StartUploadAsync(request, OwnerId);
            return Ok(result);
        }


        [HttpPost("chunk")]
        [Consumes("multipart/form-data")]  // ✅ tells Swagger this is a form upload
        public async Task<IActionResult> UploadChunk([FromForm] UploadChunkRequestDto request)
        {
            var result = await _uploadService.UploadChunkAsync(
                request.SessionId,
                request.ChunkIndex,
                request.Chunk,
                OwnerId);
                
            return Ok(result);
        }

        [HttpDelete("{sessionId}")]
        public async Task<IActionResult> Cancel(Guid sessionId)
        {
            await _uploadService.CancelUploadAsync(sessionId, OwnerId);
            return NoContent();
        }
    }
}