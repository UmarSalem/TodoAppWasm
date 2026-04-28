using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;
using Shared.Models;

namespace HttpClients.ClientInterfaces
{
    public interface ITodoService
    {
        Task CreateAsync(TodoCreationDto dto);
        Task<ICollection<Todo>> GetAsync(
        string? userName,
        int? userId,
        bool? completedStatus,
        string? titleContains,
        string? descriptionContains = null
    );
        Task UpdateAsync(TodoUpdateDto dto);
        Task<TodoBasicDto> GetByIdAsync(int id);
        Task DeleteAsync(int id);

    }

}
