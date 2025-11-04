using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using HttpClients.ClientInterfaces;
using Shared.DTOs;

namespace HttpClients.Implementations
{
    public class TodoHttpClient : ITodoService
    {
        private readonly HttpClient client;
        public TodoHttpClient(HttpClient _client)
        { this.client = _client; }

        public async Task CreateAsync(TodoCreationDto dto)
        {
           HttpResponseMessage response = await client.PostAsJsonAsync("/todos", dto);
            if (!response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                throw new Exception(content);
            }
        }
    }
}
