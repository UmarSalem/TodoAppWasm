using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;

namespace HttpClients.ClientInterfaces
{
    public interface IUserService
    {
        Task<UserReadDto> Create(UserCreationDto userCreationDto);
        Task<UserLoginResponseDto> LoginAsync(UserLoginDto userLoginDto);
        Task<IEnumerable<UserReadDto>> AsyncGetUsers(string? usernameContains = null);
    }
}
