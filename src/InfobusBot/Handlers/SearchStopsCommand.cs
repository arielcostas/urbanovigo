using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using FuzzySharp;
using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Vigo360.VitrApi.Fetcher;
using Vigo360.VitrApi.Fetcher.Models;

namespace Vigo360.VitrApi.TelegramBot.Handlers;

public class SearchStopsCommand(HttpClient http, IMemoryCache cache) : ICommand
{
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

        var stopsFetcher = new StopsFetcher(http, cache);
        var stops = await stopsFetcher.FetchStopsAsync();
        var searchResults = Process.ExtractTop(query, stops.Select(p => $"{p.Id} {p.Name}")).ToList();

        if (searchResults.Count == 0)
        {
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                replyToMessageId: message.MessageId,
                text: "No se ha encontrado ninguna parada con ese nombre.",
                parseMode: ParseMode.Html
            );
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine("Se han encontrado las siguientes paradas:");

        DetailedStop? first = null;

        foreach (var results in searchResults)
        {
            var idValue = int.Parse(results.Value.Split(' ')[0]);
            var stop = stops.First(p => p.Id == idValue);
            sb.AppendLine($"<pre>{stop.Name} ({stop.Id})</pre>");

            first ??= stop;
        }

        if (first is null)
        {
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                replyToMessageId: message.MessageId,
                text: "No se ha encontrado ninguna parada con ese nombre.",
                parseMode: ParseMode.Html
            );
            return;
        }

        await client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            replyToMessageId: message.MessageId,
            text: sb.ToString(),
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardMarkup(
                searchResults.Select(result =>
                {
                    var code = Convert.ToInt32(result.Value.Split(' ')[0]);
                    var parada = stops.First(p => p.Id == code);
                    return new KeyboardButton($"/parada {parada.Id}");
                })
            )
        );

        await client.SendVenueAsync(
            chatId: message.Chat.Id,
            latitude: first.Latitude,
            longitude: first.Longitude,
            title: first.Name,
            address: first.Name
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