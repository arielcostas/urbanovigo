using System.Text;
using System.Text.Json.Serialization;
using Costasdev.VigoTransitApi;
using Costasdev.VigoTransitApi.Types;
using FuzzySharp;
using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Costasdev.VigoTransitTelegramBot.Handlers;

public class SearchStopsCommand(VigoTransitApiClient apiClient, IMemoryCache cache) : ICommand
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

        var allStopList = await cache.GetOrCreateAsync("stops", async entry =>
        {
            entry.AbsoluteExpiration = DateTimeOffset.Now.AddHours(6);
            return await apiClient.GetStops();
        }) ?? [];
        var searchResults = Process.ExtractTop(query, allStopList.Select(p => $"{p.StopId} {p.Name}")).ToList();

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

        Stop? first = null;

        foreach (var results in searchResults)
        {
            var idValue = int.Parse(results.Value.Split(' ')[0]);
            var stop = allStopList.First(p => p.StopId == idValue);
            sb.AppendLine($"<pre>{stop.Name} ({stop.StopId})</pre>");

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
                    var stop = allStopList.First(p => p.StopId == code);
                    return new KeyboardButton($"/parada {stop.StopId}");
                })
            )
        );

        await client.SendVenueAsync(
            chatId: message.Chat.Id,
            latitude: (double)first.Latitude,
            longitude: (double)first.Longitude,
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