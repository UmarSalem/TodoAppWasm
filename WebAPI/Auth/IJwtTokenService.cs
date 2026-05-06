using Shared.DTOs;
using Shared.Models;

namespace WebAPI.Auth
{
    public interface IJwtTokenService
    {
        UserLoginResponseDto CreateLoginResponse(User user);
    }
}
