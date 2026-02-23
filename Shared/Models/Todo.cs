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
        public User Owner { get; }
        [MaxLength(50)]
        public string Title { get; set; }
        public bool IsCompleted { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public Todo( User owner, string title) { Owner = owner; Title = title; }

        private Todo() { }


    }
}
