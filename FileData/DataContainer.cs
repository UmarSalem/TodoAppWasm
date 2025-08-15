using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;

namespace FileData
{
    public class DataContainer
    {
        public ICollection<User> Users { get; set; }
        public ICollection<Todo> Todos { get; set; }
    }
}
