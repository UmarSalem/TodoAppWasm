using FileData;
using FileData.DAOs;
using Shared.DTOs;
using Shared.Models;
using Xunit;

namespace Tests;

public class TodoFileDaoTests
{
    [Fact]
    public async Task GetAsync_ReturnsAllTodos_WhenNoFilterIsProvided()
    {
        await RunInTempDirectoryAsync(async context =>
        {
            SeedTodos(context);
            var dao = new TodoFileDao(context);

            IEnumerable<Todo> todos = await dao.GetAsync(new SearchTodoParametersDto(null, null, null, null, null));

            Assert.Equal(3, todos.Count());
        });
    }

    [Fact]
    public async Task GetAsync_FiltersTodos_ByUserId()
    {
        await RunInTempDirectoryAsync(async context =>
        {
            SeedTodos(context);
            var dao = new TodoFileDao(context);

            IEnumerable<Todo> todos = await dao.GetAsync(new SearchTodoParametersDto(null, 2, null, null, null));

            Todo todo = Assert.Single(todos);
            Assert.Equal("Submit portfolio", todo.Title);
        });
    }

    [Fact]
    public async Task GetAsync_FiltersTodos_ByCompletedStatus()
    {
        await RunInTempDirectoryAsync(async context =>
        {
            SeedTodos(context);
            var dao = new TodoFileDao(context);

            IEnumerable<Todo> todos = await dao.GetAsync(new SearchTodoParametersDto(null, null, true, null, null));

            Assert.Equal(new[] { "Buy groceries" }, todos.Select(todo => todo.Title));
        });
    }

    [Fact]
    public async Task GetAsync_FiltersTodos_ByTitleContains()
    {
        await RunInTempDirectoryAsync(async context =>
        {
            SeedTodos(context);
            var dao = new TodoFileDao(context);

            IEnumerable<Todo> todos = await dao.GetAsync(new SearchTodoParametersDto(null, null, null, "portfolio", null));

            Todo todo = Assert.Single(todos);
            Assert.Equal("Submit portfolio", todo.Title);
        });
    }

    [Fact]
    public async Task GetAsync_FiltersTodos_ByDescriptionContains()
    {
        await RunInTempDirectoryAsync(async context =>
        {
            SeedTodos(context);
            var dao = new TodoFileDao(context);

            IEnumerable<Todo> todos = await dao.GetAsync(new SearchTodoParametersDto(null, null, null, null, "milk"));

            Todo todo = Assert.Single(todos);
            Assert.Equal("Buy groceries", todo.Title);
        });
    }

    private static void SeedTodos(FileContext context)
    {
        User john = new() { Id = 1, UserName = "john" };
        User sarah = new() { Id = 2, UserName = "sarah" };

        context.Users.Add(john);
        context.Users.Add(sarah);
        context.Todos.Add(new Todo(john, "Buy groceries", "Remember milk") { Id = 1, IsCompleted = true });
        context.Todos.Add(new Todo(john, "Book dentist", "Annual checkup") { Id = 2 });
        context.Todos.Add(new Todo(sarah, "Submit portfolio", "Backend user stories") { Id = 3 });
    }

    private static async Task RunInTempDirectoryAsync(Func<FileContext, Task> test)
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        string originalDir = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(tempDir);

        try
        {
            await test(new FileContext());
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDir);
            Directory.Delete(tempDir, true);
        }
    }
}
