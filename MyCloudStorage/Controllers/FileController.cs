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
            var result = await _fileService.CreateFile(request, ownerId);
            return CreatedAtAction(nameof(GetById), new { fileId = result.Id }, result);
        }

        [HttpGet("{fileId}")]
        public async Task<IActionResult> GetById(Guid fileId)
        {
            var result = await _fileService.GetFileById(fileId, ownerId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetByFolder([FromQuery] Guid? folderId)
        {
            
            var result = await _fileService.GetFilesByFolder(ownerId, folderId);
            return Ok(result);
            
        }

        [HttpPatch("{fileId}/rename")]
        public async Task<IActionResult> Rename(Guid fileId, [FromBody] RenameFileRequestDto request)
        {
            var result = await _fileService.RenameFile(ownerId, fileId, request);
            return Ok(result);
        }

        [HttpPatch("move")]
        public async Task<IActionResult> Move([FromBody] MoveFileRequestDto request)
        {
            var result = await _fileService.MoveFile(request, ownerId);
            return Ok(result);
        }

        [HttpDelete("{fileId}")]
        public async Task<IActionResult> Delete(Guid fileId)
        {
            var deleted = await _fileService.DeleteFile(ownerId, fileId);
            return deleted ? NoContent() : NotFound(new { error = "File not found." });
        }

        [HttpGet("{fileId}/download")]
        public async Task<IActionResult> Download(Guid fileId)
        {
            var (stream, fileType, fileName) = await _fileService.DownloadFileAsync(fileId, ownerId);   
                return File(stream, fileType, fileName);
        }


    }
}