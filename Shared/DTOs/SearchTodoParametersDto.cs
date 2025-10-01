using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class SearchTodoParametersDto
    {
        public string? Username { get; set; }
        public int ? UserId { get; set; }
        public bool? CompletedStatus { get; set; }
        public string? Titlecontains { get; set; }

        public string? Descriptioncontains { get; set; }

            public string? Emailcontains { get; set; }

        public SearchTodoParametersDto (string? username, int? userId, bool? completedStatus, string? titlecontains, string? descriptioncontains, string? emailcontains)
        {
            Username = username;
            UserId = userId;
            CompletedStatus = completedStatus;
            Titlecontains = titlecontains;
            Descriptioncontains = descriptioncontains;
            Emailcontains = emailcontains;
        }
    }
}
