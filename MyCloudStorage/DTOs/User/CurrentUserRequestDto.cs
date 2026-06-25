using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.DTOs.User
{
    public class CurrentUserRequestDto
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
    }
}