using System.Text;
using Costasdev.VigoTransitApi;
using Costasdev.VigoTransitApi.Types;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Costasdev.VigoTransitTelegramBot.Handlers;

public sealed class FindStopCommand(ILogger<FindStopCommand> logger, VigoTransitApiClient apiClient) : ICommand
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

        var stopIdString = args.Length == 2 ? args[1] : args[0];

        if (!int.TryParse(stopIdString, out var stopIdNumber))
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

        StopEstimateResponse requestedStop;

        try
        {
            requestedStop = await apiClient.GetStopEstimates(stopIdNumber);
        }
        catch (Exception e)
        {
            logger.LogError(Events.NotFound, e, "Error al obtener los datos de la parada");
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                replyToMessageId: message.MessageId,
                text: "Error al obtener los datos de la parada",
                parseMode: ParseMode.Html
            );
            return;
        }

        var sb = new StringBuilder();

        sb.AppendLine($"<b>{requestedStop.Stop.Name}</b> ({requestedStop.Stop.Id})");

        sb.AppendLine("<pre>min. (dist.) | Liña => Destino</pre>");
        foreach (var estimate in requestedStop.Estimates.OrderBy(e => e.Minutes))
        {
            var paddedMinutes = estimate.Minutes.ToString().PadLeft(3, ' ');
            var paddedLineNumber = estimate.Line.PadLeft(4, ' ');
            var paddedMeters = estimate.Meters.ToString().PadLeft(5, ' ');

            sb.AppendLine($"<pre> {paddedMinutes} ({paddedMeters}) | {paddedLineNumber} => {estimate.Route}</pre>");
        }

        await client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            replyToMessageId: message.MessageId,
            text: sb.ToString(),
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardMarkup(
                new KeyboardButton(stopIdString)
            )
        );
    }
}