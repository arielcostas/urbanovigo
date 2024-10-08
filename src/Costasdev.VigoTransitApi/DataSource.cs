﻿using System.Text;
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
            return await GetDataWithParams<T>(queryParams);
        }
        
        internal async Task<T?> GetDataWithParams<T>(IDictionary<string, string> parameters) where T : class
        {
            var queryString = MapToQueryString(parameters);
            var response = await _httpClient.GetAsync($"{BaseUrl}?{queryString}");

            if (!response.IsSuccessStatusCode) return null;

            var rawContent = await response.Content.ReadAsByteArrayAsync();
            var content = Encoding.GetEncoding("ISO-8859-1").GetString(rawContent);
            var contentStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            
            return await JsonSerializer.DeserializeAsync<T>(contentStream);
        }
        
        private string MapToQueryString(IDictionary<string, string> parameters)
        {
            return string.Join("&", parameters.Select(x => $"{x.Key}={x.Value}"));
        }
    }
}