using System.Text.Json;

namespace Tamaris.Web.Services.DataService
{
    public class BaseDataService
    {
        protected readonly HttpClient _httpClient;
        protected readonly JsonSerializerOptions _options;

        public BaseDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        protected async Task<T> GetResultAsync<T>(string url) where T : class
        {
            var response = await _httpClient.GetStreamAsync(url);

            if (response.Length > 0)
                return await JsonSerializer.DeserializeAsync<T>(response, _options);
            else
                return null;
        }
    }
}
