using System.Text;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Vigo360.InfobusBot.Data;

namespace Vigo360.InfobusBot.Handlers;

public sealed class ParadaCommandHandler(ILogger<ParadaCommandHandler> logger, HttpClient http) : ICommandHandler
{
    public async Task Handle(Message message, ITelegramBotClient client)
    {
        var args = message.Text!.Split(' ');

        if (args.Length > 2)
        {
            logger.LogWarning(Events.BadMessage, "Se han especificado más de dos argumentos");
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                replyToMessageId: message.MessageId,
                text: "Debes especificar solo una parada",
                parseMode: ParseMode.Html
            );
            return;
        }

        var id = args.Length == 2 ? args[1] : args[0];

        if (!int.TryParse(id, out _))
        {
            logger.LogWarning(Events.BadMessage, "El id de la parada no es un número válido");
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                replyToMessageId: message.MessageId,
                text: "El id de la parada debe ser un número válido",
                parseMode: ParseMode.Html
            );
            return;
        }

        var paradaSolicitada = await LoadStopData(id);

        if (paradaSolicitada is null)
        {
            logger.LogWarning(Events.NotFound, "No se ha encontrado la parada o ningún paso próximo");
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                replyToMessageId: message.MessageId,
                text: "No se ha encontrado la parada o ningún paso próximo",
                parseMode: ParseMode.Html
            );
            return;
        }

        var sb = new StringBuilder();

        sb.AppendLine($"<b>{paradaSolicitada.Name}</b> ({paradaSolicitada.Id})");

        sb.AppendLine($"<pre>Min | Liña => Destino</pre>");
        foreach (var paso in paradaSolicitada.Arrivals)
        {
            var minutosPadded = paso.EstimatedMinutes.PadLeft(3, ' ');
            var lineaPadded = paso.Line.PadLeft(4, ' ');

            sb.AppendLine($"<pre>{minutosPadded} | {lineaPadded} => {paso.Headsign}</pre>");
        }

        await client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            replyToMessageId: message.MessageId,
            text: sb.ToString(),
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardMarkup(
                new KeyboardButton("/parada " + paradaSolicitada.Id)
            )
        );
    }

    private async Task<Stop?> LoadStopData(string stopId)
    {
        var response = await http.GetAsync($"/Default.aspx?parada={stopId}");
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError(
                Events.BotError, "HttpError {Status}: {Body}",
                response.StatusCode, body
            );
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
            return new List<Arrival>();
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