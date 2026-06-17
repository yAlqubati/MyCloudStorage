using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.Application.Interfaces
{
    public interface IFileValidatorService
    {
        Task ValidationAsync(string filePath, string fileName, long fileSize, string fileType);
    }
}