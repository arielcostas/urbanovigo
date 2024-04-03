using HtmlAgilityPack;
using Vigo360.VitrApi.Fetcher.Models;

namespace Vigo360.VitrApi.Fetcher;

public class ArrivalFetcher(HttpClient http)
{
    public async Task<Stop?> FetchArrivalsAsync(string stopId)
    {
        var response = await http.GetAsync($"http://infobus.vitrasa.es:8002/Default.aspx?parada={stopId}");
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var doc = new HtmlDocument();
        doc.LoadHtml(body);

        var name = doc.DocumentNode.SelectSingleNode("//*[@id=\"lblNombre\"]");
        var arrivals = GetNextArrivals(doc);

        return new Stop(
            stopId,
            name.InnerText,
            arrivals.ToArray()
        );
        
    }
    
    private static List<Arrival> GetNextArrivals(HtmlDocument doc)
    {
        var rows = doc.DocumentNode.SelectNodes("//*[@id=\"GridView1\"]/tr");

        if (rows is null)
        {
            return [];
        }

        var arrivals = new List<Arrival>();

        foreach (var row in rows)
        {
            var cells = row.SelectNodes("td");

            if (cells is null || cells.Count != 3)
            {
                continue;
            }

            var line = cells[0].InnerText;
            var headsign = cells[1].InnerText;
            var estimate = cells[2].InnerText;

            arrivals.Add(new Arrival(line, headsign, estimate));
        }

        return arrivals;
    }

}