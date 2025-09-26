using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;

namespace Application.DAOInterfaces
{
    public interface ITodoDao
    {
        Task<Todo> CreateAsync(Todo todo);
    }
}
