using Application.Exceptions;
using Application.LogicInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Auth;
using Shared.DTOs;
using Shared.Models;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly ITodoLogic todoLogic;

        public TodosController(ITodoLogic todoLogic)
        {
            this.todoLogic = todoLogic;
        }

        [HttpPost]
        [ProducesResponseType(typeof(TodoReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TodoReadDto>> CreateAsync([FromBody] TodoCreationDto dto)
        {
            int userId = GetCurrentUserId();

            // The client may send ownerId, but after JWT login the server trusts the token.
            // This prevents one user from creating todos for another user by changing JSON.
            TodoCreationDto secureDto = new(userId, dto.Title, dto.Description);
            Todo created = await todoLogic.CreateAsync(secureDto);

            // Return a read DTO instead of the EF/domain model so the API contract stays stable
            // even if the internal database model changes later.
            TodoReadDto response = new(
                created.Id,
                created.OwnerId,
                created.Title,
                created.IsCompleted,
                created.Description);

            return Created($"/todos/{created.Id}", response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TodoReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<TodoReadDto>>> GetAsync(
            [FromQuery] string? userName,
            [FromQuery] int? userId,
            [FromQuery] bool? completedStatus,
            [FromQuery] string? titleContains,
            [FromQuery] string? descriptionContains)
        {
            bool isAdmin = User.IsInRole(UserRoles.Admin);
            int currentUserId = GetCurrentUserId();

            // Admins can inspect broader data. Normal users are always limited to their own todos.
            SearchTodoParametersDto parameters = isAdmin
                ? new(userName, userId, completedStatus, titleContains, descriptionContains)
                : new(null, currentUserId, completedStatus, titleContains, descriptionContains);

            var todos = await todoLogic.GetAsync(parameters);

            // Controllers translate application results into API responses; filtering and
            // business decisions stay in the application/data layers.
            IEnumerable<TodoReadDto> response = todos.Select(todo => new TodoReadDto(
                todo.Id,
                todo.OwnerId,
                todo.Title,
                todo.IsCompleted,
                todo.Description));

            return Ok(response);
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateAsync([FromBody] TodoUpdateDto dto)
        {
            TodoReadDto existing = await todoLogic.GetByIdAsync(dto.Id);
            EnsureCanAccessTodo(existing);

            if (!User.IsInRole(UserRoles.Admin))
            {
                // Normal users cannot transfer todos to another account. Ownership comes
                // from the token, not from the JSON body.
                dto.OwnerId = null;
            }

            await todoLogic.UpdateAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAsync([FromRoute] int id)
        {
            TodoReadDto existing = await todoLogic.GetByIdAsync(id);
            EnsureCanAccessTodo(existing);

            await todoLogic.DeleteAsync(id);

            // DELETE has no response body on success; the status code is enough for the client.
            return NoContent();
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TodoReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TodoReadDto>> GetById([FromRoute] int id)
        {
            // The middleware handles NotFoundException, so this method can stay focused
            // on the happy path: ask the application layer for one todo and return it.
            TodoReadDto result = await todoLogic.GetByIdAsync(id);
            EnsureCanAccessTodo(result);
            return Ok(result);
        }

        private int GetCurrentUserId()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int parsedUserId))
            {
                throw new AppValidationException("Authenticated user id is missing from the token.");
            }

            return parsedUserId;
        }

        private void EnsureCanAccessTodo(TodoReadDto todo)
        {
            if (User.IsInRole(UserRoles.Admin) || todo.OwnerId == GetCurrentUserId())
            {
                return;
            }

            // Return not found instead of forbidden so users cannot probe whether another
            // user's todo id exists.
            throw new NotFoundException($"Todo with id {todo.Id} was not found.");
        }
    }
}
