using System.Text.Json;

namespace Costasdev.VigoTransitApi
{
    internal class DataSource
    {
        private const string BaseUrl = "https://datos.vigo.org/vci_api_app/api2.jsp";
        
        private readonly HttpClient _httpClient;
        public DataSource(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        internal async Task<T?> GetData<T>(string tipo) where T : class
        {
            var queryParams = new Dictionary<string, string> { { "tipo", tipo } };
            return await GetDataParams<T>(queryParams);
        }
        
        internal async Task<T?> GetDataParams<T>(IDictionary<string, string> parameters) where T : class
        {
            var queryString = MapToQueryString(parameters);
            var response = await _httpClient.GetAsync($"{BaseUrl}?{queryString}");

            if (!response.IsSuccessStatusCode) return null;
            
            var content = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(content);
        }
        
        private string MapToQueryString(IDictionary<string, string> parameters)
        {
            return string.Join("&", parameters.Select(x => $"{x.Key}={x.Value}"));
        }
    }
}