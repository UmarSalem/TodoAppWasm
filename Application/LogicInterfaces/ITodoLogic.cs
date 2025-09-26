using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;
using Shared.Models;

namespace Application.LogicInterfaces
{
    public interface ITodoLogic
    {
        Task<Todo> CreateAsync(TodoCreationDto dto);
    }
}
