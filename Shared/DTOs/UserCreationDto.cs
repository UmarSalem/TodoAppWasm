using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class UserCreationDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(15)]
        public string UserName { get; set; }

        public string? Password { get; set; }

        public string? Email { get; set; }

        public string? EmailConfirmed { get; set; }

        public string? EmailConfirmedBy { get; set; }
        public string? EmailConfirmedSubject { get; set; }

        public string? EmailConfirmedBody { get; set; }

        [JsonConstructor]
        public UserCreationDto(String userName, String? password, String? email)

        {
            UserName = userName;
            Password = password;
            Email = email;
        }

        //public UserCreationDto(String userName)

        //{
        //    UserName = userName;
        //}





    }
}
