using System.Text;
using HtmlAgilityPack;

namespace BotVitrasa.Handlers;

public sealed class ParadaCommandHandler : ICommandHandler
{
    private static SocketsHttpHandler _handler = new()
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(15)
    };

    public async Task<string> Handle(string[] args)
    {
        if (args.Length != 1)
            return "Debes especificar solo una parada";

        if (!int.TryParse(args[0], out _))
        {
            return "El id de la parada debe ser un número válido";
        }
        
        var paradaSolicitada = await GetParada(args[0]);

        if (paradaSolicitada is null)
        {
            return "No se ha encontrado la parada o ningún paso próximo";
        }

        var sb = new StringBuilder();
        
        sb.AppendLine($"<b>{paradaSolicitada.Nombre}</b> ({paradaSolicitada.Id})");
        
        sb.AppendLine($"<pre>Min | Liña => Destino</pre>");
        foreach (var paso in paradaSolicitada.Pasos)
        {
            var minutosPadded = paso.Minutos.PadLeft(3, ' ');
            var lineaPadded = paso.Linea.PadLeft(4, ' ');
            
            sb.AppendLine($"<pre>{minutosPadded} | {lineaPadded} => {paso.Destino}</pre>");
        }
        
        return sb.ToString();
    }

    private static async Task<Parada?> GetParada(string idParada)
    {
        HttpClient cli = new(_handler)
        {
            BaseAddress = new Uri("http://infobus.vitrasa.es:8002")
        };

        var response = await cli.GetAsync($"/Default.aspx?parada={idParada}");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error: {response.StatusCode}");
            return null;
        }

        var html = await response.Content.ReadAsStringAsync();

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var nombre = doc.DocumentNode.SelectSingleNode("//*[@id=\"lblNombre\"]");
        var pasos = GetProximosPasos(doc);

        return new Parada(
            idParada,
            nombre.InnerText,
            pasos.ToArray()
        );
    }

    private static List<Paso> GetProximosPasos(HtmlDocument doc)
    {
        var rows = doc.DocumentNode.SelectNodes("//*[@id=\"GridView1\"]/tr");

        if (rows is null)
        {
            Console.WriteLine("No se ha encontrado la tabla de próximos pasos");
            return new List<Paso>();
        }

        var pasos = new List<Paso>();

        var first = true;
        foreach (var row in rows)
        {
            if (first)
            {
                first = false;
                continue;
            }

            var cells = row.SelectNodes("td");

            if (cells is null || cells.Count != 3)
            {
                continue;
            }

            var linea = cells[0].InnerText;
            var destino = cells[1].InnerText;
            var minutos = cells[2].InnerText;

            pasos.Add(new Paso(linea, destino, minutos));
        }

        return pasos;
    }
}

record Parada(string Id, string Nombre, Paso[] Pasos);

record Paso(string Linea, string Destino, string Minutos);