using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.Domain.Entities
{
    public enum UploadStatus
    {
        Active,
        Completed,
        VirusDetected,
        Failed
    }
}