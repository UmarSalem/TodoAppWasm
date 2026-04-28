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
    public class UserHttpClient : IUserService
    {
        private readonly HttpClient _client;

        public UserHttpClient (HttpClient client    )
        {
            _client = client;
        }

               public async Task<User> Create(UserCreationDto userCreationDto)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("/users",userCreationDto);
            string result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(result);
            }
            User user = JsonSerializer.Deserialize<User>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true

            })!;
            return user;
                    }
        public async Task<IEnumerable<User>> AsyncGetUsers(string? usernameContains = null)
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

            IEnumerable<User> users = JsonSerializer.Deserialize<IEnumerable<User>>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
            return users;
        }
        
        

    }
}
