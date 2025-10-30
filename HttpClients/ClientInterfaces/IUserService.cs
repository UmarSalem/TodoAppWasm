using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;
using Shared.Models;

namespace HttpClients.ClientInterfaces
{
    public interface IUserService
    {
        Task<User> Create(UserCreationDto userCreationDto);
    }
}
