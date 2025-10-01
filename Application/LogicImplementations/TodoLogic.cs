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

        public async Task<Todo> CreateAsync(TodoCreationDto dto)
        {
            User? user = await userDao.GetByIdAsync(dto.OwnerId);
            if (user == null)
            {
                throw new Exception($"User with id {dto.OwnerId} was not found.");
            }

            ValidateTodo(dto);
            Todo todo = new Todo(user, dto.Title);
            Todo created = await todoDao.CreateAsync(todo);
            return created;
        }

        private void ValidateTodo(TodoCreationDto dto)
        {
            if (string.IsNullOrEmpty(dto.Title)) throw new Exception("Title cannot be empty.");
            // other validation stuff
        }
    }
}
