using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Application.Exceptions;
using Application.LogicImplementations;
using Application.DAO_interfaces;
using Shared.Models;
using Shared.DTOs;

namespace Tests;

/// <summary>
/// Unit tests for <see cref="UserLogic"/> covering user creation scenarios.
/// </summary>
public class UserLogicTests
{
    private static readonly Pbkdf2PasswordHasher PasswordHasher = new();

    /// <summary>
    /// Simple in-memory implementation of <see cref="IUserDao"/> used for testing.
    /// </summary>
    private class FakeUserDao : IUserDao
    {
        public IList<User> Users { get; } = new List<User>();

        public Task<User> CreateAsync(User user)
        {
            // Simulate auto-increment ID assignment
            user.Id = Users.Any() ? Users.Max(u => u.Id) + 1 : 1;
            Users.Add(user);
            return Task.FromResult(user);
        }

        public Task<User?> GetByUsernameAsync(string userName)
        {
            User? user = Users.FirstOrDefault(u =>
                u.UserName != null &&
                u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }

        public Task<IEnumerable<User>> GetAllAsync(SearchUserParametersDto searchParameters)
        {
            IEnumerable<User> result = Users.AsEnumerable();

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
            User? user = Users.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        }
    }

    
   
    /// <summary>
    /// Creates a new user when the username is available.
    /// </summary>
    [Fact]
    public async Task CreateAsync_CreatesUser_WhenUsernameAvailable()
    {
        var dao = new FakeUserDao();
        var logic = new UserLogic(dao, PasswordHasher);
        var dto = new UserCreationDto("john", "Password123!");

        User created = await logic.CreateAsync(dto);

        Assert.Equal("john", created.UserName);
        Assert.NotEqual("Password123!", created.PasswordHash);
        Assert.True(PasswordHasher.Verify("Password123!", created.PasswordHash));
        Assert.Single(dao.Users);
    }

    /// <summary>
    /// Throws an exception if the username is already taken.
    /// </summary>
    [Fact]
    public async Task CreateAsync_Throws_WhenUsernameTaken()
    {
        var dao = new FakeUserDao();
        dao.Users.Add(new User { UserName = "john" });
        var logic = new UserLogic(dao, PasswordHasher);
        var dto = new UserCreationDto("john", "Password123!");

        await Assert.ThrowsAsync<ConflictException>(() => logic.CreateAsync(dto));
    }

    /// <summary>
    /// Throws an exception if the provided username is too short.
    /// </summary>
    [Fact]
    public async Task CreateAsync_Throws_WhenUsernameTooShort()
    {
        var dao = new FakeUserDao();
        var logic = new UserLogic(dao, PasswordHasher);
        var dto = new UserCreationDto("ab", "Password123!");

        var ex = await Assert.ThrowsAsync<AppValidationException>(() => logic.CreateAsync(dto));
        Assert.Equal("Username must be at least 3 characters!", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenUsernameIsEmpty()
    {
        var logic = new UserLogic(new FakeUserDao(), PasswordHasher);
        var dto = new UserCreationDto("", "Password123!");

        var ex = await Assert.ThrowsAsync<AppValidationException>(() => logic.CreateAsync(dto));
        Assert.Equal("Username is required.", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenUsernameIsTooLong()
    {
        var logic = new UserLogic(new FakeUserDao(), PasswordHasher);
        var dto = new UserCreationDto("abcdefghijklmnop", "Password123!");

        var ex = await Assert.ThrowsAsync<AppValidationException>(() => logic.CreateAsync(dto));
        Assert.Equal("Username must be less than 16 characters!", ex.Message);
    }

    [Fact]
    public async Task GetAsync_ReturnsAllUsers_WhenNoFilterIsProvided()
    {
        var dao = new FakeUserDao();
        dao.Users.Add(new User { Id = 1, UserName = "john" });
        dao.Users.Add(new User { Id = 2, UserName = "sarah" });
        var logic = new UserLogic(dao, PasswordHasher);

        IEnumerable<User> users = await logic.GetAsync(new SearchUserParametersDto());

        Assert.Equal(2, users.Count());
    }

    [Fact]
    public async Task GetAsync_FiltersUsers_ByUsernameContains()
    {
        var dao = new FakeUserDao();
        dao.Users.Add(new User { Id = 1, UserName = "john" });
        dao.Users.Add(new User { Id = 2, UserName = "sarah" });
        dao.Users.Add(new User { Id = 3, UserName = "joanna" });
        var logic = new UserLogic(dao, PasswordHasher);

        IEnumerable<User> users = await logic.GetAsync(new SearchUserParametersDto("jo"));

        Assert.Equal(new[] { "john", "joanna" }, users.Select(user => user.UserName));
    }

    [Fact]
    public async Task GetAsync_FiltersUsers_ByUserId()
    {
        var dao = new FakeUserDao();
        dao.Users.Add(new User { Id = 1, UserName = "john" });
        dao.Users.Add(new User { Id = 2, UserName = "sarah" });
        var logic = new UserLogic(dao, PasswordHasher);

        IEnumerable<User> users = await logic.GetAsync(new SearchUserParametersDto(userId: 2));

        User user = Assert.Single(users);
        Assert.Equal("sarah", user.UserName);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenPasswordTooShort()
    {
        var logic = new UserLogic(new FakeUserDao(), PasswordHasher);
        var dto = new UserCreationDto("john", "short");

        var ex = await Assert.ThrowsAsync<AppValidationException>(() => logic.CreateAsync(dto));
        Assert.Equal("Password must be at least 8 characters!", ex.Message);
    }

    [Fact]
    public async Task LoginAsync_ReturnsUser_WhenPasswordMatches()
    {
        var dao = new FakeUserDao();
        dao.Users.Add(new User
        {
            Id = 1,
            UserName = "john",
            PasswordHash = PasswordHasher.Hash("Password123!")
        });
        var logic = new UserLogic(dao, PasswordHasher);

        User user = await logic.LoginAsync(new UserLoginDto("john", "Password123!"));

        Assert.Equal(1, user.Id);
        Assert.Equal("john", user.UserName);
    }

    [Fact]
    public async Task LoginAsync_Throws_WhenPasswordDoesNotMatch()
    {
        var dao = new FakeUserDao();
        dao.Users.Add(new User
        {
            Id = 1,
            UserName = "john",
            PasswordHash = PasswordHasher.Hash("Password123!")
        });
        var logic = new UserLogic(dao, PasswordHasher);

        var ex = await Assert.ThrowsAsync<AppValidationException>(
            () => logic.LoginAsync(new UserLoginDto("john", "WrongPassword123!")));

        Assert.Equal("Invalid username or password.", ex.Message);
    }
}
