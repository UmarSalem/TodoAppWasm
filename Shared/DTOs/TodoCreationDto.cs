using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class TodoCreationDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "OwnerId must be greater than zero.")]
        public int OwnerId { get; }

        [Required]
        [MaxLength(50)]
        public string Title { get; }

        [MaxLength(200)]
        public string? Description { get; }

        public TodoCreationDto(int ownerId, string title, string? description = null)
        {
            OwnerId = ownerId;
            Title = title;
            Description = description;
        }
    }
}
