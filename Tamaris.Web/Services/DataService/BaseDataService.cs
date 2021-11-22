using System.Text;
using System.Text.Json;
using Tamaris.Domains.DataShaping;

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

        protected async Task<Tuple<T, PaginationHeader>> GetResultAsync<T>(string url, int pageIndex, int pageSize) where T : class
        {
            try
            {
                PaginationHeader pagination = null;
                url += $"?PageIndex={pageIndex}&PageSize={pageSize}";
                var request = new HttpRequestMessage(HttpMethod.Get, url);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                var headers = response.Headers.FirstOrDefault();

                if (headers.Key == "x-pagination" && !string.IsNullOrEmpty(headers.Value.FirstOrDefault()))
                {
                    var json = headers.Value.FirstOrDefault();
                    pagination = JsonSerializer.Deserialize<PaginationHeader>(json, _options);
                }

                var stream = await response.Content.ReadAsStreamAsync();

                if (stream.Length > 0)
                {
                    var result = await JsonSerializer.DeserializeAsync<T>(stream, _options);
                    return Tuple.Create(result, pagination);
                }
                else
                    return null;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
