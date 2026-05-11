namespace Shared.DTOs
{
    using Shared.Auth;

    public class UserReadDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }

        public UserReadDto(int id, string userName, string role = UserRoles.User)
        {
            Id = id;
            UserName = userName;
            Role = role;
        }
    }
}
