using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.Configuration
{
    public class CorsSettings
    {
        public const string SectionName = "Cors";
        public List<string> AllowedOrigins { get; set; } = new();
    }
}