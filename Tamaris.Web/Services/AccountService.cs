using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using Tamaris.Domains.Authorization;

namespace Tamaris.Web.Services
{
    public class AccountService: IAccountService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        private readonly CustomStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;


        public AccountService(HttpClient httpClient, CustomStateProvider authStateProvider, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _authStateProvider = authStateProvider;
            _localStorage = localStorage;
        }


        public async Task<AuthorizationResponse> Login(LoginRequest loginRequest)
        {
            var result = await _httpClient.PostAsJsonAsync("Authorization/Login", loginRequest);

            if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                throw new Exception(await result.Content.ReadAsStringAsync());

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<AuthorizationResponse>(content, _options);

            if (!result.IsSuccessStatusCode)
                return response;

            await _localStorage.SetItemAsync("authToken", response.Token);
            _authStateProvider.NotifyUserAuthentication(loginRequest.UserName);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", response.Token);
            // response.IsAuthorized = true;

            // result.EnsureSuccessStatusCode();

            return response;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _authStateProvider.NotifyUserLogout();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<RegisterResponse> Register(RegisterRequest registerRequest)
        {
            //var result = await _httpClient.PostAsJsonAsync("Authorization/Register", registerRequest);
            //if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
            //    throw new Exception(await result.Content.ReadAsStringAsync());
            //result.EnsureSuccessStatusCode();


            //var content = JsonSerializer.Serialize(registerRequest);
            //var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            var registrationResult = await _httpClient.PostAsJsonAsync("Authorization/Register", registerRequest);
            var registrationContent = await registrationResult.Content.ReadAsStringAsync();

            if (!registrationResult.IsSuccessStatusCode && !string.IsNullOrEmpty(registrationContent))
            {
                var result = JsonSerializer.Deserialize<RegisterResponse>(registrationContent, _options);
                return result;
            }

            return new RegisterResponse { IsRegistrationSuccessful = true };
        }

        public async Task<string> ForgotPassword(ForgotPasswordRequest request)
        {
            var result = await _httpClient.PostAsJsonAsync("Authorization/ForgotPassword", request);
            var content = await result.Content.ReadAsStringAsync();

            if (!result.IsSuccessStatusCode && !string.IsNullOrEmpty(content))
                return content;

            return "";
        }


        public async Task<string> ResetPassword(ResetPasswordRequest request)
        {
            var result = await _httpClient.PostAsJsonAsync("Authorization/ResetPassword", request);
            var content = await result.Content.ReadAsStringAsync();

            if (!result.IsSuccessStatusCode && !string.IsNullOrEmpty(content))
                return content;

            return "";
        }
    }
}

