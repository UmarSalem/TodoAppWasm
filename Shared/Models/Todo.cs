using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public User Owner { get; }
        public string Title { get; set; }
        public bool IsCompleted { get; }

        public string Description { get; set; }

        public Todo( User owner, string title) { Owner = owner; Title = title; }



    }
}
