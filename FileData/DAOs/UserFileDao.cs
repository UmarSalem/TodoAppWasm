using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DAO_interfaces;
using Shared.Models;

namespace FileData.DAOs
{ 
public class UserFileDao : IUserDao
{
    private readonly FileContext context;

    public UserFileDao(FileContext context)
    {
        this.context = context;
    }

        public Task<User> CreateAsync(User user)
        {
            int userId = 1;
            if (context.Users.Any())
            {
                userId = context.Users.Max(u => u.Id);
                userId++;
            }

            user.Id = userId;

            context.Users.Add(user);
            context.SaveChanges();

            return Task.FromResult(user);
        }

        public Task<User?> GetByUsernameAsync(string userName)
        {
            User? existing = context.Users.FirstOrDefault(u =>
                u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
            );
            return Task.FromResult(existing);
        }

    }
}