using Application.DAO_interfaces;
using Application.DAOInterfaces;
using Application.Exceptions;
using Application.LogicImplementations;
using Shared.DTOs;
using Shared.Models;
using Xunit;

namespace Tests;

public class TodoLogicTests
{
    [Fact]
    public async Task UpdateAsync_UpdatesTodo_WhenRequestIsValid()
    {
        var todoDao = new FakeTodoDao();
        var userDao = new FakeUserDao();
        userDao.Users.Add(new User { Id = 1, UserName = "john" });
        todoDao.Todos.Add(new Todo(1, "Old title", "Old description") { Id = 1 });
        var logic = new TodoLogic(todoDao, userDao);

        await logic.UpdateAsync(new TodoUpdateDto(1)
        {
            Title = "New title",
            Description = "New description",
            IsCompleted = true
        });

        Todo updated = Assert.Single(todoDao.Todos);
        Assert.Equal("New title", updated.Title);
        Assert.Equal("New description", updated.Description);
        Assert.True(updated.IsCompleted);
        Assert.Equal(1, updated.OwnerId);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFound_WhenTodoDoesNotExist()
    {
        var logic = new TodoLogic(new FakeTodoDao(), new FakeUserDao());

        await Assert.ThrowsAsync<NotFoundException>(() => logic.UpdateAsync(new TodoUpdateDto(99)));
    }

    [Fact]
    public async Task UpdateAsync_ChangesOwner_WhenOwnerExists()
    {
        var todoDao = new FakeTodoDao();
        var userDao = new FakeUserDao();
        userDao.Users.Add(new User { Id = 1, UserName = "john" });
        userDao.Users.Add(new User { Id = 2, UserName = "sarah" });
        todoDao.Todos.Add(new Todo(1, "Existing todo") { Id = 1 });
        var logic = new TodoLogic(todoDao, userDao);

        await logic.UpdateAsync(new TodoUpdateDto(1)
        {
            OwnerId = 2
        });

        Todo updated = Assert.Single(todoDao.Todos);
        Assert.Equal(2, updated.OwnerId);
        Assert.Equal("sarah", updated.Owner?.UserName);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFound_WhenNewOwnerDoesNotExist()
    {
        var todoDao = new FakeTodoDao();
        todoDao.Todos.Add(new Todo(1, "Existing todo") { Id = 1 });
        var logic = new TodoLogic(todoDao, new FakeUserDao());

        await Assert.ThrowsAsync<NotFoundException>(() => logic.UpdateAsync(new TodoUpdateDto(1)
        {
            OwnerId = 77
        }));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsConflict_WhenCompletedTodoIsMarkedIncomplete()
    {
        var todoDao = new FakeTodoDao();
        var userDao = new FakeUserDao();
        userDao.Users.Add(new User { Id = 1, UserName = "john" });
        todoDao.Todos.Add(new Todo(1, "Existing todo") { Id = 1, IsCompleted = true });
        var logic = new TodoLogic(todoDao, userDao);

        await Assert.ThrowsAsync<ConflictException>(() => logic.UpdateAsync(new TodoUpdateDto(1)
        {
            IsCompleted = false
        }));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsValidation_WhenTitleIsEmpty()
    {
        var todoDao = new FakeTodoDao();
        var userDao = new FakeUserDao();
        userDao.Users.Add(new User { Id = 1, UserName = "john" });
        todoDao.Todos.Add(new Todo(1, "Existing todo") { Id = 1 });
        var logic = new TodoLogic(todoDao, userDao);

        await Assert.ThrowsAsync<AppValidationException>(() => logic.UpdateAsync(new TodoUpdateDto(1)
        {
            Title = ""
        }));
    }

    private class FakeTodoDao : ITodoDao
    {
        public IList<Todo> Todos { get; } = new List<Todo>();

        public Task<Todo> CreateAsync(Todo todo)
        {
            Todos.Add(todo);
            return Task.FromResult(todo);
        }

        public Task DeleteAsync(int id)
        {
            Todo? todo = Todos.FirstOrDefault(t => t.Id == id);
            if (todo != null)
            {
                Todos.Remove(todo);
            }

            return Task.CompletedTask;
        }

        public Task<IEnumerable<Todo>> GetAsync(SearchTodoParametersDto searchParameterDto)
        {
            return Task.FromResult(Todos.AsEnumerable());
        }

        public Task<Todo?> GetByIdAsync(int todoId)
        {
            return Task.FromResult(Todos.FirstOrDefault(t => t.Id == todoId));
        }

        public Task UpdateAsync(Todo todo)
        {
            Todo? existing = Todos.FirstOrDefault(t => t.Id == todo.Id);
            if (existing != null)
            {
                Todos.Remove(existing);
            }

            Todos.Add(todo);
            return Task.CompletedTask;
        }
    }

    private class FakeUserDao : IUserDao
    {
        public IList<User> Users { get; } = new List<User>();

        public Task<User> CreateAsync(User user)
        {
            Users.Add(user);
            return Task.FromResult(user);
        }

        public Task<IEnumerable<User>> GetAllAsync(SearchUserParametersDto searchUserParametersDto)
        {
            return Task.FromResult(Users.AsEnumerable());
        }

        public Task<User?> GetByIdAsync(int id)
        {
            return Task.FromResult(Users.FirstOrDefault(user => user.Id == id));
        }

        public Task<User?> GetByUsernameAsync(string userName)
        {
            return Task.FromResult(Users.FirstOrDefault(user => user.UserName == userName));
        }
    }
}
