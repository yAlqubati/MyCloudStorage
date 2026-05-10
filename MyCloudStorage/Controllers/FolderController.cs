using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCloudStorage.Application.Interfaces;
using MyCloudStorage.DTOs.Folder;

namespace MyCloudStorage.Controllers
{
    [ApiController]
    [Route("api/folders")]
    [Authorize]
    public class FolderController : ControllerBase
    {
        private readonly IFolderService _folderService;
        private readonly ILogger<FolderController> _logger;
        private string ownerId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;



        public FolderController(IFolderService folderService, ILogger<FolderController> logger)
        {
            _folderService = folderService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateFolderRequestDto request)
        {
            _logger.LogInformation("OwnerId {owerid}", ownerId);
            var result = await _folderService.CreateFolder(request, ownerId);
            return CreatedAtAction(nameof(GetChildren), new { parentFolderId = result.Id }, result);
        }

        [HttpGet("root")]
        public async Task<IActionResult> GetRoot()
        {
            var result = await _folderService.GetRootFolders(ownerId);
            return Ok(result);
        }

        [HttpGet("{parentFolderId}/children")]
        public async Task<IActionResult> GetChildren(Guid parentFolderId)
        {
            var result = await _folderService.GetChildFolders(ownerId, parentFolderId);
            return Ok(result);
        }

        [HttpPatch("{folderId}/rename")]
        public async Task<IActionResult> Rename(Guid folderId, [FromBody] RenameFolderRequestDto request)
        {
            var result = await _folderService.RenameFolder(ownerId, folderId, request);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpDelete("{folderId}")]
        public async Task<IActionResult> Delete(Guid folderId)
        {
            var deleted = await _folderService.DeleteFolder(ownerId, folderId);
            return deleted ? NoContent() : NotFound();
        }


    }
}