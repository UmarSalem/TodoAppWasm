using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DAO_interfaces;
using Application.LogicInterfaces;
using Shared.DTOs;
using Shared.Models;

namespace Application.LogicImplementations
{
    public class UserLogic : IUserLogic
    {
        private readonly IUserDao userDao;

        public UserLogic(IUserDao userDao)
        {
            this.userDao = userDao;
        }

        public async Task<User> CreateAsync(UserCreationDto usertoCreate)
  
 
               {
                    User? existing = await userDao.GetByUsernameAsync(usertoCreate.UserName);
                        if (existing != null)
                         throw new Exception("Username already taken!");

                                 ValidateData(usertoCreate);
                                 User toCreate = new User
                                {
                                    UserName = usertoCreate.UserName
                                  };
    
                             User created = await userDao.CreateAsync(toCreate);
    
                                return created;
                }

        private static void ValidateData(UserCreationDto userToCreate)
        {
            string userName = userToCreate.UserName;

            if (userName.Length < 3)
                throw new Exception("Username must be at least 3 characters!");

            if (userName.Length > 15)
                throw new Exception("Username must be less than 16 characters!");
        }

        public Task<IEnumerable<User>> GetAsync(SearchUserParametersDto searchParametersDto)
        {
            return userDao.GetAllAsync(searchParametersDto);
        }
    }
}
