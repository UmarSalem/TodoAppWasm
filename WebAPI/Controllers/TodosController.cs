using Application.LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Models;

namespace WebAPI.Controllers
{
    [ApiController]
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
            Todo created = await todoLogic.CreateAsync(dto);

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
            SearchTodoParametersDto parameters = new(userName, userId, completedStatus, titleContains, descriptionContains);
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
            return Ok(result);
        }
    }
}
