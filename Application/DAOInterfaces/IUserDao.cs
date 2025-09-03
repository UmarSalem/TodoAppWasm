using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;

namespace Application.DAO_interfaces
{
    public interface IUserDao
    {
        Task<User> CreateAsync(User user);
        Task<User?> GetByUsernameAsync(String userName);
        

    }
}
