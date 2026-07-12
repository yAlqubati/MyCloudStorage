using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCloudStorage.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string body);
    }
}