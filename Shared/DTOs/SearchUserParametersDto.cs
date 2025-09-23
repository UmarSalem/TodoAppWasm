using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class SearchUserParametersDto
    {
        //   public string? UsernameContains { get; }


        //public SearchUserParametersDto(string? usernameContains)
        //   {
        //       UsernameContains = usernameContains;
        //    }

        public string? UsernameContains { get; }
        public int? UserId { get; }

        public SearchUserParametersDto(string? usernameContains = null, int? userId = null)
        {
            UsernameContains = usernameContains;
            UserId = userId;
        }







    }
}
