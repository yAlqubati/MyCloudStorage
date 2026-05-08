using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCloudStorage.Application.Interfaces;
using MyCloudStorage.Domain.Entities;
using MyCloudStorage.DTOs.File;

namespace MyCloudStorage.Controllers
{
    [ApiController]
    [Route("api/files")]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<FileController> _logger;
        private string ownerId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public FileController(IFileService fileService, ILogger<FileController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateFileRequestDto request)
        {
            try
            {
                var result = await _fileService.CreateFile(request, ownerId);
                return CreatedAtAction(nameof(GetById), new { fileId = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{fileId}")]
        public async Task<IActionResult> GetById(int fileId)
        {
            try
            {
                var result = await _fileService.GetFileById(fileId, ownerId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetByFolder([FromQuery] int? folderId)
        {
            try
            {
                // normalize 0 to null here too, in case it comes via query string
                var normalizedId = (folderId == 0) ? null : folderId;
                var result = await _fileService.GetFilesByFolder(ownerId, normalizedId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpPatch("{fileId}/rename")]
        public async Task<IActionResult> Rename(int fileId, [FromBody] RenameFileRequestDto request)
        {
            try
            {
                var result = await _fileService.RenameFile(ownerId, fileId, request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{fileId}")]
        public async Task<IActionResult> Delete(int fileId)
        {
            var deleted = await _fileService.DeleteFile(ownerId, fileId);
            return deleted ? NoContent() : NotFound(new { error = "File not found." });
        }




    }
}