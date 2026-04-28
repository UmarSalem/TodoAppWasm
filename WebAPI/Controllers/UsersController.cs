using Application.Exceptions;
using Application.LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserLogic userLogic;

        public UsersController(IUserLogic userLogic)
        {
            this.userLogic = userLogic;
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserReadDto>> CreateAsync([FromBody] UserCreationDto dto)
        {
            try
            {
                User user = await userLogic.CreateAsync(dto);
                UserReadDto response = new(user.Id, user.UserName);
                return Created($"/users/{user.Id}", response);
            }
            catch (AppValidationException e)
            {
                return BadRequest(new ApiErrorDto(e.Message));
            }
            catch (ConflictException e)
            {
                return Conflict(new ApiErrorDto(e.Message));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, new ApiErrorDto("An unexpected error occurred while creating the user."));
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAsync(
            [FromQuery] string? usernameContains,
            [FromQuery] int? userId)
        {
            try
            {
                SearchUserParametersDto parameters = new(usernameContains, userId);
                IEnumerable<User> users = await userLogic.GetAsync(parameters);
                IEnumerable<UserReadDto> response = users.Select(user => new UserReadDto(user.Id, user.UserName));
                return Ok(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, new ApiErrorDto("An unexpected error occurred while fetching users."));
            }
        }
    }
}
