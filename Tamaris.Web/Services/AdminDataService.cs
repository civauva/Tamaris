using System.Net.Http.Json;
using System.Text.Json;
using Tamaris.Domains.Admin;

namespace Tamaris.Web.Services
{
    public class AdminDataService: IAdminDataService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public AdminDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IEnumerable<UserForSelect>> GetAllUsers()
        {
            return await JsonSerializer.DeserializeAsync<IEnumerable<UserForSelect>>
                (await _httpClient.GetStreamAsync($"Admin/Users"), _options);
        }

        public async Task<UserForSelect> GetUserById(int userId)
        {
            return await JsonSerializer.DeserializeAsync<UserForSelect>
                (await _httpClient.GetStreamAsync($"Admin/Users/{userId}"), _options);
        }

        public async Task<UserForSelect> GetUserByUsername(string userName)
        {
            return await JsonSerializer.DeserializeAsync<UserForSelect>
                (await _httpClient.GetStreamAsync($"Admin/Users/{userName}"), _options);
        }

        public async Task<UserForSelect> AddUser(UserForInsert user)
        {
            var registrationResult = await _httpClient.PostAsJsonAsync("Admin/Users", user);
            var registrationContent = await registrationResult.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<UserForSelect>(registrationContent, _options);
            return result;


            //if (!registrationResult.IsSuccessStatusCode && !string.IsNullOrEmpty(registrationContent))
            //{
            //    var result = JsonSerializer.Deserialize<UserForSelect>(registrationContent, _options);
            //    return result;
            //}

            //return new RegisterResponse { IsRegistrationSuccessful = true };
        }

        public async Task<UserForSelect> DeleteUser(string username)
        {
            var deletionResult = await _httpClient.DeleteAsync($"Admin/Users/{username}");
            var deletionContent = await deletionResult.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<UserForSelect>(deletionContent, _options);
            return result;
        }
    }
}

