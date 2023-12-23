using MyBook.Domain.Models;
using MyBook.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBook.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<APIResponse<string>> RegisterAsync(ApplicationUser user, string password);
    }
}
