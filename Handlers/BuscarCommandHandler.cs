using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FuzzySharp;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotVitrasa.Handlers;

public class BuscarCommandHandler : ICommandHandler
{
    private readonly HttpClient _httpClient = new();
    private readonly List<ParadaBus> _paradas;

    public BuscarCommandHandler()
    {
        var rt = _httpClient.GetAsync("https://datos.vigo.org/data/transporte/paradas.json");
        var content = rt.Result.Content.ReadAsStringAsync();
        _paradas = JsonSerializer.Deserialize<List<ParadaBus>>(content.Result) ?? new List<ParadaBus>();
    }

    public async Task Handle(Message message, ITelegramBotClient client)
    {
        var args = message.Text!.Split(' ')[1..];

        if (args.Length == 0)
        {
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                replyToMessageId: message.MessageId,
                text: "No se ha especificado ningún término de búsqueda.",
                parseMode: ParseMode.Html
            );
            return;
        }

        var query = string.Join(' ', args);

        var resultados = Process.ExtractTop(query, _paradas.Select(p => p.Nombre));

        if (resultados == null)
        {
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                replyToMessageId: message.MessageId,
                text: "No se ha encontrado ninguna parada con ese nombre.",
                parseMode: ParseMode.Html
            );
            return;
        }

        var r = resultados.ToArray();

        StringBuilder sb = new();
        sb.AppendLine("Se han encontrado las siguientes paradas:");

        foreach (var resultado in r)
        {
            var parada = _paradas.First(p => p.Nombre == resultado.Value);
            sb.AppendLine($"<pre>{parada.Nombre} ({parada.Id})</pre>");
        }

        await client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            replyToMessageId: message.MessageId,
            text: sb.ToString(),
            parseMode: ParseMode.Html
        );

        var p0 = _paradas.First(p => p.Nombre == r[0].Value);
        await client.SendVenueAsync(
            chatId: message.Chat.Id,
            latitude: p0.Latitud,
            longitude: p0.Longitud,
            title: p0.Nombre,
            address: p0.Nombre
        );
    }
}

#pragma warning disable 8618
public class ParadaBus
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("lineas")] public string LineasRaw { get; set; }
    [JsonPropertyName("lat")] public double Latitud { get; set; }
    [JsonPropertyName("lon")] public double Longitud { get; set; }
    [JsonPropertyName("nombre")] public string Nombre { get; set; }
    public List<string> Lineas => LineasRaw.Split(',').Select(s => s.Trim()).ToList();
}