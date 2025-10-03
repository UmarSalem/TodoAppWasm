using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;
using Shared.Models;

namespace Application.DAOInterfaces
{
    public interface ITodoDao
    {
        Task<Todo> CreateAsync(Todo todo);
        Task<IEnumerable<Todo>> GetAsync(SearchTodoParametersDto searchParameterDto);
        Task UpdateAsync(Todo todo);

        Task<Todo?> GetByIdAsync(int todoId);
    }
}
