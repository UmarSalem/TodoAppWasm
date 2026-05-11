using HttpClients.ClientInterfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.DTOs;

namespace BlazorApp.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserService userService;
        private readonly ITokenStore tokenStore;
        private readonly AuthenticationStateProvider authenticationStateProvider;

        public AuthService(
            IUserService userService,
            ITokenStore tokenStore,
            AuthenticationStateProvider authenticationStateProvider)
        {
            this.userService = userService;
            this.tokenStore = tokenStore;
            this.authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<UserLoginResponseDto> LoginAsync(UserLoginDto dto)
        {
            UserLoginResponseDto response = await userService.LoginAsync(dto);
            await tokenStore.SetTokenAsync(response.Token);

            // Tell Blazor that AuthorizeView/NavMenu should refresh immediately after login.
            ((JwtAuthenticationStateProvider)authenticationStateProvider).NotifyUserChanged();
            return response;
        }

        public async Task LogoutAsync()
        {
            await tokenStore.ClearTokenAsync();
            ((JwtAuthenticationStateProvider)authenticationStateProvider).NotifyUserChanged();
        }
    }
}
