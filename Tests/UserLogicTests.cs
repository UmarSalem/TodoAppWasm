using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
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
    /// <summary>
    /// Simple in-memory implementation of <see cref="IUserDao"/> used for testing.
    /// </summary>
    private class FakeUserDao : IUserDao
    {
        public IList<User> Users { get; } = new List<User>();

        public Task<User> CreateAsync(User user)
        {
            Users.Add(user);
            return Task.FromResult(user);
        }

        public Task<User?> GetByUsernameAsync(string userName)
        {
            User? user = Users.FirstOrDefault(u => u.UserName == userName);
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
        var logic = new UserLogic(dao);
        var dto = new UserCreationDto("john", "pass", "mail@example.com");

        User created = await logic.CreateAsync(dto);

        Assert.Equal("john", created.UserName);
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
        var logic = new UserLogic(dao);
        var dto = new UserCreationDto("john", "pass", "mail@example.com");

        await Assert.ThrowsAsync<Exception>(() => logic.CreateAsync(dto));
    }

    /// <summary>
    /// Throws an exception if the provided username is too short.
    /// </summary>
    [Fact]
    public async Task CreateAsync_Throws_WhenUsernameTooShort()
    {
        var dao = new FakeUserDao();
        var logic = new UserLogic(dao);
        var dto = new UserCreationDto("ab", "pass", "mail@example.com");

        var ex = await Assert.ThrowsAsync<Exception>(() => logic.CreateAsync(dto));
        Assert.Equal("Username must be at least 3 characters!", ex.Message);
    }
}
