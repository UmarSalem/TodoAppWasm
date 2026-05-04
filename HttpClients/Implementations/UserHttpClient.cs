using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HttpClients.ClientInterfaces;
using Shared.DTOs;

namespace HttpClients.Implementations
{
    public class UserHttpClient : IUserService
    {
        private readonly HttpClient _client;

        public UserHttpClient (HttpClient client    )
        {
            _client = client;
        }

               public async Task<UserReadDto> Create(UserCreationDto userCreationDto)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("/users",userCreationDto);
            string result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(result);
            }
            UserReadDto user = JsonSerializer.Deserialize<UserReadDto>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true

            })!;
            return user;
                    }
        public async Task<IEnumerable<UserReadDto>> AsyncGetUsers(string? usernameContains = null)
        {
            string uri = "/users";
            if (!string.IsNullOrEmpty(usernameContains))
            {
                uri += $"?usernameContains={usernameContains}";
            }
            HttpResponseMessage response = await _client.GetAsync(uri);
            string result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(result);
            }

            IEnumerable<UserReadDto> users = JsonSerializer.Deserialize<IEnumerable<UserReadDto>>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
            return users;
        }
        
        

    }
}
