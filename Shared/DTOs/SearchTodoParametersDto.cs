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
        public string? TitleContains { get; set; }

        public string? DescriptionContains { get; set; }

        public SearchTodoParametersDto (string? userName, int? userId, bool? completedStatus, string? titleContains, string? descriptionContains)
        {
            UserName = userName;
            UserId = userId;
            CompletedStatus = completedStatus;
            TitleContains = titleContains;
            DescriptionContains = descriptionContains;
        }
    }
}
