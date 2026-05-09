using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.DTOs
{
    public class UserLoginDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [JsonConstructor]
        public UserLoginDto(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}
