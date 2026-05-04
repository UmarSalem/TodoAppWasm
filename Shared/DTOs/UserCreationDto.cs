using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.DTOs
{
    public class UserCreationDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(15)]
        public string UserName { get; set; }

        [JsonConstructor]
        public UserCreationDto(string userName)
        {
            UserName = userName;
        }
    }
}
