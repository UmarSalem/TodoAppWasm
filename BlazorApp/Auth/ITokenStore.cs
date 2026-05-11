namespace BlazorApp.Auth
{
    public interface ITokenStore
    {
        Task<string?> GetTokenAsync();
        Task SetTokenAsync(string token);
        Task ClearTokenAsync();
    }
}
