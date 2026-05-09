using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;
using Shared.Models;

namespace Application.LogicInterfaces
{
    public interface IUserLogic
    {

        public Task<User> CreateAsync(UserCreationDto userToCreate);
        public Task<IEnumerable<User>> GetAsync(SearchUserParametersDto searchUserParametersDto);
        public Task<User> LoginAsync(UserLoginDto dto);

    }
}
