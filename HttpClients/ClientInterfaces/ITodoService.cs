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
    }
}
