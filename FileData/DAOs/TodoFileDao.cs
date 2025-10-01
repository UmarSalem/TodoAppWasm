using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DAOInterfaces;
using Shared.Models;

namespace FileData.DAOs
{
    public class TodoFileDao : ITodoDao
    {
        private readonly FileContext context;

        public TodoFileDao(FileContext context)
        {
            this.context = context;
        }

        public Task<Todo> CreateAsync(Todo todo)
        {
            int id = 1;
            if(context.Todos.Any())
            {
                id = context.Todos.Max(t => t.Id);
                id++;
            }
            todo.Id = id;

        context.Todos.Add(todo);
            context.SaveChanges();
            return Task.FromResult(todo);
        }
    }
}
