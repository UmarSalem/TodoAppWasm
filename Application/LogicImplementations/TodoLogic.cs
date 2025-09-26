using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DAO_interfaces;
using Application.DAOInterfaces;
using Application.LogicInterfaces;
using Shared.DTOs;
using Shared.Models;

namespace Application.LogicImplementations
{
    public class TodoLogic : ITodoLogic
    {
        private readonly ITodoDao todoDao;
        private readonly IUserDao userDao;

        public TodoLogic(ITodoDao todoDao, IUserDao userDao)
        {
            this.todoDao = todoDao;
            this.userDao = userDao;
        }

        public Task<Todo> CreateAsync(TodoCreationDto todo)
        {
            throw new NotImplementedException();
        }
    }
}
