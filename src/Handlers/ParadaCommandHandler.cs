using System.Net.Http.Headers;
using System.Text;
using BotVitrasa.Data;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotVitrasa.Handlers;

public sealed class ParadaCommandHandler : ICommandHandler
{
    private readonly ILogger<ParadaCommandHandler> _logger;
    private readonly HttpClient _http;

    public ParadaCommandHandler(ILogger<ParadaCommandHandler> logger, HttpClient http)
    {
        _logger = logger;
        _http = http;
    }

    public async Task Handle(Message message, ITelegramBotClient client)
    {
        var args = message.Text!.Split(' ');

        if (args.Length > 2)
        {
            _logger.LogWarning(Events.BadMessage, "Se han especificado más de dos argumentos");
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                replyToMessageId: message.MessageId,
                text: "Debes especificar solo una parada",
                parseMode: ParseMode.Html
            );
        }

        if (args.Length > 2)
        {
            _logger.LogWarning(Events.BadMessage, "Se han especificado más de dos argumentos");
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
            _logger.LogWarning(Events.BadMessage, "El id de la parada no es un número válido");
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                replyToMessageId: message.MessageId,
                text: "El id de la parada debe ser un número válido",
                parseMode: ParseMode.Html
            );
            return;
        }

        var paradaSolicitada = await GetParada(id);

        if (paradaSolicitada is null)
        {
            _logger.LogWarning(Events.NotFound, "No se ha encontrado la parada o ningún paso próximo");
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                replyToMessageId: message.MessageId,
                text: "No se ha encontrado la parada o ningún paso próximo",
                parseMode: ParseMode.Html
            );
            return;
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

    private async Task<Parada?> GetParada(string idParada)
    {
        var response = await _http.GetAsync($"/Default.aspx?parada={idParada}");
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                Events.BotError, "HttpError {Status}: {Body}",
                response.StatusCode, body
            );
            return null;
        }

        var doc = new HtmlDocument();
        doc.LoadHtml(body);

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