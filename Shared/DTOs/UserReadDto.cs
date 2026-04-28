namespace Shared.DTOs
{
    public class UserReadDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public UserReadDto(int id, string userName)
        {
            Id = id;
            UserName = userName;
        }
    }
}
