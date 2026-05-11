using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.Application.Interfaces
{
    public interface IStorageService
    {
        Task<string> SaveFileAsync(Stream fileStream, string storageKey);
        Task DeleteFileAsync(string storageKey);
        Task<Stream> GetFileAsync(string storageKey);
    }
}