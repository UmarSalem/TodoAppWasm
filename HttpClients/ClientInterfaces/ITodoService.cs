using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;

namespace HttpClients.ClientInterfaces
{
    public interface ITodoService
    {
        Task CreateAsync(TodoCreationDto dto);
        Task<ICollection<TodoReadDto>> GetAsync(
        string? userName,
        int? userId,
        bool? completedStatus,
        string? titleContains,
        string? descriptionContains = null
    );
        Task UpdateAsync(TodoUpdateDto dto);
        Task<TodoReadDto> GetByIdAsync(int id);
        Task DeleteAsync(int id);

    }

}
