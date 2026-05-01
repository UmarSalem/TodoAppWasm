using Application.Exceptions;
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
            try
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
            catch (AppValidationException e)
            {
                return BadRequest(new ApiErrorDto(e.Message));
            }
            catch (NotFoundException e)
            {
                return NotFound(new ApiErrorDto(e.Message));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, new ApiErrorDto("An unexpected error occurred while creating the todo."));
            }
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
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, new ApiErrorDto("An unexpected error occurred while fetching todos."));
            }
        }

        [HttpPatch]
        public async Task<ActionResult> UpdateAsync([FromBody] TodoUpdateDto dto)
        {
            try
            {
                await todoLogic.UpdateAsync(dto);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, new ApiErrorDto("An unexpected error occurred while updating the todo."));
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAsync([FromRoute] int id)
        {
            try
            {
                await todoLogic.DeleteAsync(id);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, new ApiErrorDto("An unexpected error occurred while deleting the todo."));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TodoBasicDto>> GetById([FromRoute] int id)
        {
            try
            {
                TodoBasicDto result = await todoLogic.GetByIdAsync(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, new ApiErrorDto("An unexpected error occurred while fetching the todo."));
            }
        }
    }
}
