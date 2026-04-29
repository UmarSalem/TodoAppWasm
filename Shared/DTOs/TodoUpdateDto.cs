using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs
{
    public class TodoUpdateDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than zero.")]
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "OwnerId must be greater than zero.")]
        public int? OwnerId { get; set; }

        [MaxLength(50)]
        public string? Title { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public bool? IsCompleted { get; set; }

        public TodoUpdateDto(int id)
        {
            Id = id;
        }
    }
}
