using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MyCloudStorage.Application.Interfaces;
using Serilog;

namespace MyCloudStorage.Application.Services
{
    public class FileValidationService : IFileValidatorService
    {
        private readonly ILogger<IFileValidatorService> _logger;
        private readonly long _maxFileSizeBytes = 500 * 1024 * 1024;

        private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/gif", "image/webp",
            "application/pdf",
            "text/plain", "text/csv",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.ms-excel",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/zip", "application/x-zip-compressed",
            "video/mp4", "video/webm",
            "audio/mpeg", "audio/wav"
        };

        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp",
            ".pdf",
            ".txt", ".csv",
            ".doc", ".docx",
            ".xls", ".xlsx",
            ".zip",
            ".mp4", ".webm",
            ".mp3", ".wav"
        };

        // magic bytes 
        private static readonly Dictionary<string, byte[][]> MagicBytes = new()
        {
            [".jpg"]  = new[] { new byte[] { 0xFF, 0xD8, 0xFF } },
            [".png"]  = new[] { new byte[] { 0x89, 0x50, 0x4E, 0x47 } },
            [".gif"]  = new[] { new byte[] { 0x47, 0x49, 0x46, 0x38 } },
            [".pdf"]  = new[] { new byte[] { 0x25, 0x50, 0x44, 0x46 } },
            [".zip"]  = new[] { new byte[] { 0x50, 0x4B, 0x03, 0x04 } },
            [".mp4"]  = new[] { new byte[] { 0x00, 0x00, 0x00, 0x18, 0x66, 0x74, 0x79, 0x70 },
                                new byte[] { 0x00, 0x00, 0x00, 0x20, 0x66, 0x74, 0x79, 0x70 } },
        };


        public FileValidationService(ILogger<IFileValidatorService> logger)
        {
            _logger = logger;
        }


        public async Task ValidationAsync(string filePath, string fileName, long fileSize, string fileType)
        {
            ValidateSize(fileSize);
            ValidateExtension(fileName);
            ValidateMimeType(fileType, fileName);
            await ValidateMagicBytesAsync(filePath, fileName);

            _logger.LogInformation("File validation passed for {FileName}", fileName);
        }


        private void ValidateSize(long fileSize)
        {
            if (fileSize <= 0)
                throw new ValidationException("File is empty.");

            if (fileSize > _maxFileSizeBytes)
                throw new ValidationException(
                    $"File size {fileSize / 1024 / 1024}MB exceeds the maximum allowed size of {_maxFileSizeBytes / 1024 / 1024}MB.");
        }

        private void ValidateExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName);

            if (string.IsNullOrEmpty(extension))
                throw new ValidationException("File must have an extension.");

            if (!AllowedExtensions.Contains(extension))
                throw new ValidationException($"File type '{extension}' is not allowed.");
        }

        private void ValidateMimeType(string fileType, string fileName)
        {
            if (!AllowedMimeTypes.Contains(fileType))
                throw new ValidationException($"MIME type '{fileType}' is not allowed.");

            // Cross-check: extension and MIME type must agree
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var mismatch = (extension, fileType) switch
            {
                (".jpg" or ".jpeg", var m) when !m.Contains("jpeg") => true,
                (".png", var m) when m != "image/png"               => true,
                (".pdf", var m) when m != "application/pdf"         => true,
                (".mp4", var m) when m != "video/mp4"               => true,
                _                                                    => false
            };

            if (mismatch)
                throw new ValidationException(
                    $"File extension '{extension}' does not match declared type '{fileType}'.");
        }

        private async Task ValidateMagicBytesAsync(string filePath, string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            // Only check types we have signatures for
            if (!MagicBytes.TryGetValue(extension, out var signatures))
                return;

            // Read just the first 12 bytes — enough for any magic signature
            var header = new byte[12];
            await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            await stream.ReadAsync(header);

            var isValid = signatures.Any(sig =>
                header.Take(sig.Length).SequenceEqual(sig));

            if (!isValid)
                throw new ValidationException(
                    $"File content does not match its extension '{extension}'. The file may be corrupted or renamed.");
        }
    }
}