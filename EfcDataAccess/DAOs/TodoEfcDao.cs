using Application.DAOInterfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        private readonly TodoContext _context;
        public TodoEfcDao(TodoContext context) 
        {
         _context = context;
        }
        public async Task<Todo> CreateAsync(Todo todo)
        {
          EntityEntry<Todo>  added = _context.Todos.Add(todo);
          await _context.SaveChangesAsync();
            return added.Entity;
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
