using Shared.DTOs;

namespace BlazorApp.Auth
{
    public interface IAuthService
    {
        Task<UserLoginResponseDto> LoginAsync(UserLoginDto dto);
        Task LogoutAsync();
    }
}
