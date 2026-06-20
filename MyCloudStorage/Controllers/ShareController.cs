using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyCloudStorage.Application.Interfaces;
using MyCloudStorage.DTOs.File;
using Superpower.Model;

namespace MyCloudStorage.Controllers
{
    [ApiController]
    [Route("api/share")]
    [Authorize]
    [EnableRateLimiting("api")]
    public class ShareController : ControllerBase
    {
        private readonly IShareFileService _shareService;

        private string ownerId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public ShareController(IShareFileService shareService)
        {
            _shareService = shareService;
        }

        [HttpPost("user")]
        public async Task<IActionResult> ShareWithUser([FromBody] CreateFileShareRequestDto request)
        {
            var result = await _shareService.ShareFile(request,ownerId);
            return Ok(result);
        }

        [HttpGet("get/{fileId}")]
        public async Task<IActionResult> GetSharesForFile(Guid fileId)
        {
            var result = await _shareService.GetSharesForFile(fileId, ownerId);
            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetSharedWithMe()
        {
            var result = await _shareService.GetFilesSharedWithMe(ownerId);
            return Ok(result);
        }

        [HttpDelete("user/{shareId}")]
        public async Task<IActionResult> RevokeShare(Guid shareId)
        {
            await _shareService.RevokeFileShare(shareId, ownerId);
            return NoContent();
        }

        [HttpGet("download/{fileId}")]
        public async Task<IActionResult> DownloadSharedFile(Guid fileId)
        {
            var (stream, contentType, fileName) =
                await _shareService.DownloadViaShareAsync(ownerId, fileId);
            return File(stream, contentType, fileName);
        }
    }
}