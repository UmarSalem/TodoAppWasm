using Application.DAO_interfaces;
using Shared.DTOs;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfcDataAccess.DAOs
{
    public class UserEfcDao : IUserDao
    {
        private readonly TodoContext context;

        public UserEfcDao (TodoContext context) { this.context = context; }
       public async Task<User> CreateAsync(User user)
{
          EntityEntry<User> newUser = await context.Users.AddAsync(user);
          await context.SaveChangesAsync();
           return newUser.Entity;
}

        public Task<IEnumerable<User>> GetAllAsync(SearchUserParametersDto searchUserParametersDto)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<User?> GetByUsernameAsync(string userName)
        {
            User? existing = await context.Users.FirstOrDefaultAsync(u =>
                u.UserName.ToLower().Equals(userName.ToLower())
            );
            return existing;
        }
    }
}
