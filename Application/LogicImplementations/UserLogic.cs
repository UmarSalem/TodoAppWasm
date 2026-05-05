using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DAO_interfaces;
using Application.Exceptions;
using Application.LogicInterfaces;
using Shared.DTOs;
using Shared.Models;

namespace Application.LogicImplementations
{
    public class UserLogic : IUserLogic
    {
        private readonly IUserDao userDao;
        private readonly IPasswordHasher passwordHasher;

        public UserLogic(IUserDao userDao, IPasswordHasher passwordHasher)
        {
            this.userDao = userDao;
            this.passwordHasher = passwordHasher;
        }

        public async Task<User> CreateAsync(UserCreationDto usertoCreate)
        {
            ValidateData(usertoCreate);

            User? existing = await userDao.GetByUsernameAsync(usertoCreate.UserName);
            if (existing != null)
                throw new ConflictException("Username already taken!");

            // Only the hash is stored. If the database is leaked, attackers still do not
            // get the user's original password.
            User toCreate = new User
            {
                UserName = usertoCreate.UserName,
                PasswordHash = passwordHasher.Hash(usertoCreate.Password)
            };

            User created = await userDao.CreateAsync(toCreate);

            return created;
        }

        public async Task<User> LoginAsync(UserLoginDto dto)
        {
            ValidateLoginData(dto);

            User? existing = await userDao.GetByUsernameAsync(dto.UserName);
            if (existing == null || !passwordHasher.Verify(dto.Password, existing.PasswordHash))
            {
                // Use one shared message so the API does not reveal whether the username
                // or the password was the incorrect part.
                throw new AppValidationException("Invalid username or password.");
            }

            return existing;
        }

        private static void ValidateData(UserCreationDto userToCreate)
        {
            string userName = userToCreate.UserName;
            string password = userToCreate.Password;

            if (string.IsNullOrWhiteSpace(userName))
                throw new AppValidationException("Username is required.");

            if (userName.Length < 3)
                throw new AppValidationException("Username must be at least 3 characters!");

            if (userName.Length > 15)
                throw new AppValidationException("Username must be less than 16 characters!");

            if (string.IsNullOrWhiteSpace(password))
                throw new AppValidationException("Password is required.");

            if (password.Length < 8)
                throw new AppValidationException("Password must be at least 8 characters!");

            if (password.Length > 100)
                throw new AppValidationException("Password must be less than 101 characters!");
        }

        private static void ValidateLoginData(UserLoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Password))
                throw new AppValidationException("Invalid username or password.");
        }

        public Task<IEnumerable<User>> GetAsync(SearchUserParametersDto searchParametersDto)
        {
            return userDao.GetAllAsync(searchParametersDto);
        }
    }
}
