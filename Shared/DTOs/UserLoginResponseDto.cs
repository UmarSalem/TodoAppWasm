namespace Shared.DTOs
{
    public class UserLoginResponseDto
    {
        public string Token { get; set; }
        public UserReadDto User { get; set; }
        public DateTime ExpiresAtUtc { get; set; }

        public UserLoginResponseDto(string token, UserReadDto user, DateTime expiresAtUtc)
        {
            Token = token;
            User = user;
            ExpiresAtUtc = expiresAtUtc;
        }
    }
}
