using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCloudStorage.Application.Interfaces;

namespace MyCloudStorage.Application.Services
{
    public class LocalStorageService : IStorageService
    {

        private readonly string _basePath;
        private readonly ILogger<LocalStorageService> _logger;

        public LocalStorageService(IConfiguration config, ILogger<LocalStorageService> logger)
        {
            _basePath = config["Storage:BasePath"] ?? "uploads";
            _logger = logger;
        }

        public async Task DeleteFileAsync(string storageKey)
        {
            var fullPath = Path.Combine(_basePath, storageKey);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("File deleted: {Path}", fullPath);

            }
            await Task.CompletedTask;
        }

        public async Task<Stream> GetFileAsync(string storageKey)
        {
            var fullPath = Path.Combine(_basePath, storageKey);
            if(!File.Exists(fullPath))
                throw new FileNotFoundException("File not found in storage ", storageKey);

            return await Task.FromResult<Stream>(
                new FileStream(fullPath, FileMode.Open, FileAccess.Read)
            );
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string storageKey)
        {
            var fullPath = Path.Combine(_basePath, storageKey);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            using var outPut = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
            await fileStream.CopyToAsync(outPut);

            _logger.LogInformation("File saved to {Path}", fullPath);
            return storageKey;
        }


    }
}