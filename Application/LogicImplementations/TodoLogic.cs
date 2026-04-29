using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DAO_interfaces;
using Application.DAOInterfaces;
using Application.Exceptions;
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
                throw new NotFoundException($"User with id {dto.OwnerId} was not found.");
            }

            ValidateTodo(dto);
            Todo todo = new Todo(dto.OwnerId, dto.Title, dto.Description);
            Todo created = await todoDao.CreateAsync(todo);
            return created;
        }

        public async Task DeleteAsync(int id)
        {
            Todo? todo = await todoDao.GetByIdAsync(id);
            if (todo == null)
            {
                throw new NotFoundException($"Todo with id {id} was not found.");
            }

            if (!todo.IsCompleted)
            {
                // This is a business rule from the todo story: only completed todos may be deleted.
                // The WebAPI middleware turns this conflict into HTTP 409 for clients.
                throw new ConflictException("Cannot delete an incomplete todo.");
            }

            await todoDao.DeleteAsync(id);
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
                throw new NotFoundException($"Todo with id {dto.Id} was not found.");
            }

            User? user = null;
            if (dto.OwnerId != null)
            {
                user = await userDao.GetByIdAsync((int)dto.OwnerId);
                if (user == null)
                {
                    throw new NotFoundException($"User with id {dto.OwnerId} was not found.");
                }
            }

            if (dto.IsCompleted != null && existing.IsCompleted && !(bool)dto.IsCompleted)
            {
                // Once a todo is completed, the app keeps that state final.
                // Throwing a typed exception lets the API return 409 Conflict instead of 500.
                throw new ConflictException("Cannot mark a completed todo as incomplete.");
            }

            // Update the tracked entity instead of creating a second object with the same Id.
            // EF Core can throw tracking errors when two instances share the same primary key.
            existing.OwnerId = dto.OwnerId ?? existing.OwnerId;
            existing.Owner = user ?? existing.Owner;
            existing.Title = dto.Title ?? existing.Title;
            existing.Description = dto.Description ?? existing.Description;
            existing.IsCompleted = dto.IsCompleted ?? existing.IsCompleted;

            ValidateTodo(existing);

            await todoDao.UpdateAsync(existing);
        }

        private void ValidateTodo(Todo dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new AppValidationException("Title is required.");

            if (dto.Title.Length > 50)
                throw new AppValidationException("Title must be 50 characters or fewer.");

            if (dto.Description?.Length > 200)
                throw new AppValidationException("Description must be 200 characters or fewer.");
        }

        private void ValidateTodo(TodoCreationDto dto)
        {
            if (dto.OwnerId <= 0)
                throw new AppValidationException("OwnerId must be greater than zero.");

            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new AppValidationException("Title is required.");

            if (dto.Title.Length > 50)
                throw new AppValidationException("Title must be 50 characters or fewer.");

            if (dto.Description?.Length > 200)
                throw new AppValidationException("Description must be 200 characters or fewer.");
        }

        public async Task<TodoBasicDto> GetByIdAsync(int id)
        {
            Todo? todo = await todoDao.GetByIdAsync(id);
            if (todo == null)
            {
                throw new Exception($"Todo with id {id} not found");
            }

            return new TodoBasicDto(todo.Id, todo.Owner?.UserName ?? string.Empty, todo.Title, todo.IsCompleted);
        }

            }
}
