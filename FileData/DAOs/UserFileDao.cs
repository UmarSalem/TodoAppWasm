using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DAO_interfaces;
using Shared.DTOs;
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


        //public Task<IEnumerable<User>> GetAllAsync(SearchUserParametersDto searchUserParametersDto)
        //{
        //    IEnumerable<User> Users = context.Users.AsEnumerable();
        //    if (searchUserParametersDto.UsernameContains != null)
        //    {
        //        Users = context.Users.Where(u => u.UserName.Contains(searchUserParametersDto.UsernameContains, StringComparison.OrdinalIgnoreCase));
        //    }
        //    return Task.FromResult(Users);
        //}

        public Task<IEnumerable<User>> GetAllAsync(SearchUserParametersDto searchParameters)
        {
            IEnumerable<User> result = context.Users.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchParameters.UsernameContains))
            {
                result = result.Where(user => user.UserName != null &&
                    user.UserName.Contains(searchParameters.UsernameContains, StringComparison.OrdinalIgnoreCase));
            }

            if (searchParameters.UserId != null)
            {
                result = result.Where(user => user.Id == searchParameters.UserId);
            }

            return Task.FromResult(result);
        }

        public Task<User?> GetByIdAsync(int id)
        {
            User? existing = context.Users.FirstOrDefault(u =>
                u.Id == id
            );
            return Task.FromResult(existing);
        }

    }
}