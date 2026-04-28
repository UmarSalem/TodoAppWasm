using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class TodoUpdateDto
    {
        public int Id { get; set; }
        public int? OwnerId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public String? UserName { get; set; }
        public bool? IsCompleted { get; set; }
        public TodoUpdateDto(int id)
        {
            Id = id;
        }

    }
}
