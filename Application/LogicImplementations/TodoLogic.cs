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

        public Task<IEnumerable<Todo>> GetAsync(SearchTodoParametersDto searchParametersDto)
        {
            return todoDao.GetAsync(searchParametersDto);
        }

        public async Task UpdateAsync(TodoUpdateDto dto)
        {
            Todo? existing = await todoDao.GetByIdAsync(dto.Id);

            if (existing == null)
            {
                throw new Exception($"Todo with ID {dto.Id} not found!");
            }

            User? user = null;
            if (dto.OwnerId != null)
            {
                user = await userDao.GetByIdAsync((int)dto.OwnerId);
                if (user == null)
                {
                    throw new Exception($"User with id {dto.OwnerId} was not found.");
                }
            }

            if (dto.IsCompleted != null && existing.IsCompleted && !(bool)dto.IsCompleted)
            {
                throw new Exception("Cannot un-complete a completed Todo");
            }

            User userToUse = user ?? existing.Owner;
            string titleToUse = dto.Title ?? existing.Title;
            bool completedToUse = dto.IsCompleted ?? existing.IsCompleted;

            Todo updated = new(userToUse, titleToUse)
            {
                IsCompleted = completedToUse,
                Id = existing.Id,
            };

            ValidateTodo(updated);

            await todoDao.UpdateAsync(updated);
        }

        private void ValidateTodo(Todo dto)
        {
            if (string.IsNullOrEmpty(dto.Title)) throw new Exception("Title cannot be empty.");
            // other validation stuff
        }

        private void ValidateTodo(TodoCreationDto dto)
        {
            if (string.IsNullOrEmpty(dto.Title)) throw new Exception("Title cannot be empty.");
            // other validation stuff
        }
    }
}
