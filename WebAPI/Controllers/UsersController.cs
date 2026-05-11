using Application.LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Models;
using WebAPI.Auth;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserLogic userLogic;
        private readonly IJwtTokenService jwtTokenService;

        public UsersController(IUserLogic userLogic, IJwtTokenService jwtTokenService)
        {
            this.userLogic = userLogic;
            this.jwtTokenService = jwtTokenService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserReadDto>> CreateAsync([FromBody] UserCreationDto dto)
        {
            User user = await userLogic.CreateAsync(dto);
            UserReadDto response = new(user.Id, user.UserName);
            return Created($"/users/{user.Id}", response);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(UserLoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserLoginResponseDto>> LoginAsync([FromBody] UserLoginDto dto)
        {
            User user = await userLogic.LoginAsync(dto);

            // Login returns a signed JWT. The client sends this token on later API calls,
            // and the server reads the user id/role from the token instead of trusting form data.
            UserLoginResponseDto response = jwtTokenService.CreateLoginResponse(user);
            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAsync(
            [FromQuery] string? usernameContains,
            [FromQuery] int? userId)
        {
            SearchUserParametersDto parameters = new(usernameContains, userId);
            IEnumerable<User> users = await userLogic.GetAsync(parameters);
            IEnumerable<UserReadDto> response = users.Select(user => new UserReadDto(user.Id, user.UserName, user.Role));
            return Ok(response);
        }
    }
}
