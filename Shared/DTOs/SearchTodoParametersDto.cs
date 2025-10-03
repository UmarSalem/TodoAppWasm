using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class SearchTodoParametersDto
    {
        public string? UserName { get; set; }
        public int ? UserId { get; set; }
        public bool? CompletedStatus { get; set; }
        public string? Titlecontains { get; set; }

        public string? Descriptioncontains { get; set; }

            public string? Emailcontains { get; set; }

        public SearchTodoParametersDto (string? userName, int? userId, bool? completedStatus, string? titlecontains, string? descriptioncontains, string? emailcontains)
        {
            UserName = userName;
            UserId = userId;
            CompletedStatus = completedStatus;
            Titlecontains = titlecontains;
            Descriptioncontains = descriptioncontains;
            Emailcontains = emailcontains;
        }
    }
}
