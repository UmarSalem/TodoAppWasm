using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class Todo
    {
        [Key]
        public int Id { get; set; }
        public User? Owner { get; set; }
        public int OwnerId { get; set; }
        [MaxLength(50)]
        public string Title { get; set; }
        public bool IsCompleted { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public Todo(int ownerId, string title, string? description = null)
        {
            OwnerId = ownerId;
            Title = title;
            Description = description;
        }

        public Todo(User owner, string title, string? description = null)
            : this(owner.Id, title, description)
        {
            Owner = owner;
        }

        private Todo() { }


    }
}
