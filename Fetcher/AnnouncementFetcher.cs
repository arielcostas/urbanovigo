using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using Vigo360.VitrApi.Fetcher.Models;

namespace Vigo360.VitrApi.Fetcher;

public class AnnouncementFetcher(HttpClient http)
{
    public async Task<List<Announcement>> FetchAnnouncementsAsync()
    {
        var response = await http.GetAsync("https://vitrasa.es/php/index.php?pag=index");
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            // TODO: Log error
            return [];
        }

        var doc = new HtmlDocument();
        doc.LoadHtml(body);

        return [];
    }
}