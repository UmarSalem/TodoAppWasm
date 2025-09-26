using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class TodoCreationDto
    {
        public int OwnerId { get; }
        public string Title { get; }

        public TodoCreationDto(int ownerId, string title)
        {
            OwnerId = ownerId;
            Title = title;
        }
    }
}
