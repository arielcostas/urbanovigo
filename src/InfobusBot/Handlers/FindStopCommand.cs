using System.Text;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Vigo360.VitrApi.Fetcher;
using Vigo360.VitrApi.Fetcher.Models;
using Vigo360.VitrApi.TelegramBot.Data;
using Vigo360.VitrApi.TelegramBot.Data.Models;
using Vigo360.VitrApi.TelegramBot;

namespace Vigo360.VitrApi.TelegramBot.Handlers;

public sealed class FindStopCommand(ILogger<FindStopCommand> logger, HttpClient http) : ICommand
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

        var fetcher = new ArrivalFetcher(http);
        var paradaSolicitada = await fetcher.FetchArrivalsAsync(id);

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
                new KeyboardButton(paradaSolicitada.Id)
            )
        );
    }

}