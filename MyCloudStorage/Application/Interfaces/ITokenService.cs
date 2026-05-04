using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCloudStorage.Domain.Entities;

namespace MyCloudStorage.Application.Interfaces
{
    public interface ITokenService
    {
        public string CreateToken(User user);
    }
}