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

        public async Task<ICollection<Todo>> GetAsync(string? userName, int? userId, bool? completedStatus, string? titleContains, string? descriptionContains = null)
        {
            string query = ConstructQuery(userName, userId, completedStatus, titleContains, descriptionContains);

            HttpResponseMessage response = await client.GetAsync("/todos" + query);
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

        private static string ConstructQuery(string? userName, int? userId, bool? completedStatus, string? titleContains, string? descriptionContains)
        {
            string query = "";
            if (!string.IsNullOrEmpty(userName))
            {
                query += $"?userName={Uri.EscapeDataString(userName)}";
            }

            if (userId != null)
            {
                query += string.IsNullOrEmpty(query) ? "?" : "&";
                query += $"userId={userId}";
            }

            if (completedStatus != null)
            {
                query += string.IsNullOrEmpty(query) ? "?" : "&";
                query += $"completedStatus={completedStatus}";
            }

            if (!string.IsNullOrEmpty(titleContains))
            {
                query += string.IsNullOrEmpty(query) ? "?" : "&";
                query += $"titleContains={Uri.EscapeDataString(titleContains)}";
            }

            if (!string.IsNullOrEmpty(descriptionContains))
            {
                query += string.IsNullOrEmpty(query) ? "?" : "&";
                query += $"descriptionContains={Uri.EscapeDataString(descriptionContains)}";
            }

            return query;
        }

        public async Task UpdateAsync(TodoUpdateDto dto)
        {
            String dtoAsJson = JsonSerializer.Serialize(dto);
            StringContent body = new StringContent(dtoAsJson, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PatchAsync("/todos", body);
            if (!response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                throw new Exception(content);
            }
        }

        public async Task<TodoBasicDto> GetByIdAsync(int id)
        {
            HttpResponseMessage response = await client.GetAsync($"/todos/{id}");
            String content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }
            TodoBasicDto todo = JsonSerializer.Deserialize<TodoBasicDto>(content,
                new JsonSerializerOptions

                {
                    PropertyNameCaseInsensitive = true
                }
                )!;
            return todo;
        }

        public async Task DeleteAsync(int id)
        {
            HttpResponseMessage response = await client.DeleteAsync($"Todos/{id}");
            if (!response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                throw new Exception(content);
            }
        }
    }
}
