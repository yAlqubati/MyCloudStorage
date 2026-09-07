using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.Configuration
{
    public class EmailSettings
    {
        public const string SectionName = "Email";

        public int Port { get; set; } = 587;
        public string From { get; set; } = string.Empty;
    }
}