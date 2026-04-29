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
            return NoContent();
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TodoBasicDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TodoBasicDto>> GetById([FromRoute] int id)
        {
            TodoBasicDto result = await todoLogic.GetByIdAsync(id);
            return Ok(result);
        }
    }
}
