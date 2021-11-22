using System.Net.Http.Json;
using System.Text.Json;
using Tamaris.Domains.Admin;
using Tamaris.Domains.DataShaping;

namespace Tamaris.Web.Services.DataService
{
    public class AdminDataService: BaseDataService, IAdminDataService
    {
        public AdminDataService(HttpClient httpClient): base(httpClient) {}

        public async Task<Tuple<IEnumerable<UserForSelect>, PaginationHeader>> GetAllUsers(int pageIndex, int pageSize)
        {
            var result = await GetResultAsync<IEnumerable<UserForSelect>>($"Admin/Users", pageIndex, pageSize);
            return result;
        }

        public async Task<IEnumerable<UserForChat>> GetAllUsersForMessaging(string excludeUsername)
        {
            return await GetResultAsync<IEnumerable<UserForChat>>($"Admin/Users/ForChat/{excludeUsername}");
        }

        public async Task<UserForSelect> GetUserById(int userId)
        {
            return await GetResultAsync<UserForSelect>($"Admin/Users/{userId}");
        }

        public async Task<UserForSelect> GetUserByUsernameAsync(string userName)
        {
            return await GetResultAsync<UserForSelect>($"Admin/Users/ByUsername/{userName}");
        }

        public async Task<UserForSelect> GetUserByEmailAsync(string userName)
        {
            return await GetResultAsync<UserForSelect>($"Admin/Users/ByEmail/{userName}");
        }

        public async Task<UserForSelect> AddUser(UserForInsert user)
        {
            var registrationResult = await _httpClient.PostAsJsonAsync("Admin/Users", user);
            var registrationContent = await registrationResult.Content.ReadAsStringAsync();

            if (registrationResult.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<UserForSelect>(registrationContent, _options);
                return result;
            }
            else
			{
                throw new Exception(registrationContent);
			}


            //if (!registrationResult.IsSuccessStatusCode && !string.IsNullOrEmpty(registrationContent))
            //{
            //    var result = JsonSerializer.Deserialize<UserForSelect>(registrationContent, _options);
            //    return result;
            //}

            //return new RegisterResponse { IsRegistrationSuccessful = true };
        }

        public async Task ModifyUser(UserForUpdate user)
        {
            var modifyResult = await _httpClient.PutAsJsonAsync($"Admin/Users/{user.Id}", user);

            if (!modifyResult.IsSuccessStatusCode)
            {
                var modifyContent = await modifyResult.Content.ReadAsStringAsync();
                throw new Exception(modifyContent);
            }

            return;
        }

        public async Task ModifyUserProfile(UserForProfileUpdate user)
        {
            var modifyResult = await _httpClient.PutAsJsonAsync($"Admin/Users/Profile/{user.Id}", user);
            // var modifyContent = await modifyResult.Content.ReadAsStringAsync();

            return;
        }


        public async Task<UserForSelect> DeleteUser(string username)
        {
            var deletionResult = await _httpClient.DeleteAsync($"Admin/Users/{username}");
            var deletionContent = await deletionResult.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<UserForSelect>(deletionContent, _options);
            return result;
        }

        public async Task<IEnumerable<RoleForSelect>> GetAllRoles()
        {
            return await GetResultAsync<IEnumerable<RoleForSelect>>($"Admin/Roles");
        }
    }
}

