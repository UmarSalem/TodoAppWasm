using Application.DAOInterfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Todo>> GetAsync(SearchTodoParametersDto searchParameterDto)
        {
            IQueryable<Todo> query = _context.Todos.Include(todo => todo.Owner).AsQueryable();

            if (!string.IsNullOrEmpty(searchParameterDto.UserName))
            {
                // we know username is unique, so just fetch the first
                query = query.Where(todo =>
                    todo.Owner.UserName.ToLower().Equals(searchParameterDto.UserName.ToLower()));
            }

            if (searchParameterDto.UserId != null)
            {
                query = query.Where(t => t.Owner.Id == searchParameterDto.UserId);
            }

            if (searchParameterDto.CompletedStatus != null)
            {
                query = query.Where(t => t.IsCompleted == searchParameterDto.CompletedStatus);
            }

            if (!string.IsNullOrEmpty(searchParameterDto.Titlecontains))
            {
                query = query.Where(t =>
                    t.Title.ToLower().Contains(searchParameterDto.Titlecontains.ToLower()));
            }

            List<Todo> result = await query.ToListAsync();
            return result;
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
