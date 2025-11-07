using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HttpClients.ClientInterfaces;
using Shared.DTOs;
using Shared.Models;

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

        public async Task<ICollection<Todo>> GetAsync(string? userName, int? userId, bool? completedStatus, string? titleContains)
        {
            HttpResponseMessage response = await client.GetAsync("/todos");
            string content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }

            ICollection<Todo> todos = JsonSerializer.Deserialize<ICollection<Todo>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
            return todos;
        }
    }
}
