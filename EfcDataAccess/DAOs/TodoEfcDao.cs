using Application.DAOInterfaces;
using Shared.DTOs;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfcDataAccess.DAOs
{
    public class TodoEfcDao : ITodoDao
    {
        public Task<Todo> CreateAsync(Todo todo)
        {
            throw new NotImplementedException();
        }
        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Todo>> GetAsync(SearchTodoParametersDto searchParameterDto)
        {
            throw new NotImplementedException();
        }

        public Task<Todo?> GetByIdAsync(int todoId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Todo todo)
        {
            throw new NotImplementedException();
        }
    }
}
