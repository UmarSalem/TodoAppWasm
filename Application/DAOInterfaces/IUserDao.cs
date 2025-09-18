using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;
using Shared.Models;

namespace Application.DAO_interfaces
{
    public interface IUserDao
    {
        Task<User> CreateAsync(User user);
        Task<User?> GetByUsernameAsync(String userName);

        //Task<User?> GetByEmailAsync(String email);

        Task<IEnumerable<User>> GetAllAsync(SearchUserParametersDto searchUserParametersDto);
        

    }
}
