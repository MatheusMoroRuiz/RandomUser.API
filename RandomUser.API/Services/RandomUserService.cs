using System.Text.Json;
using RandomUser.API.Models;

namespace RandomUser.API.Services
{
    public class RandomUserService
    {
        private readonly HttpClient _httpClient;

        public RandomUserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<User>> GetRandomUsersAsync(int count = 10)
        {
            var response = await _httpClient.GetStringAsync($"https://randomuser.me/api/?results={count}");
            var json = JsonDocument.Parse(response);

            var users = new List<User>();

            foreach (var result in json.RootElement.GetProperty("results").EnumerateArray())
            {
                users.Add(new User
                {
                    Name = $"{result.GetProperty("name").GetProperty("first").GetString()} {result.GetProperty("name").GetProperty("last").GetString()}",
                    Email = result.GetProperty("email").GetString(),
                    Gender = result.GetProperty("gender").GetString(),
                    Country = result.GetProperty("location").GetProperty("country").GetString()
                });
            }

            return users;
        }
    }
}
