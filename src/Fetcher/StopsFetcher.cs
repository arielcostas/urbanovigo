using System.Net.Http.Json;
using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using Vigo360.VitrApi.Fetcher.Models;

namespace Vigo360.VitrApi.Fetcher;

public class StopsFetcher(HttpClient http, IMemoryCache cache)
{
    private const string StopsCacheKey = "stops";
    
    public async Task<List<DetailedStop>> FetchStopsAsync()
    {
        if (cache.TryGetValue(StopsCacheKey, out List<DetailedStop>? cachedStops))
        {
            return cachedStops ?? [];
        }
        
        var rt = await http.GetAsync("https://datos.vigo.org/data/transporte/paradas.json");
        var stops = await rt.Content.ReadFromJsonAsync<List<DetailedStop>>() ?? [];

        if (stops.Count == 0)
        {
            cache.Set(StopsCacheKey, stops, TimeSpan.FromHours(1));
        }

        return stops;
    }
}