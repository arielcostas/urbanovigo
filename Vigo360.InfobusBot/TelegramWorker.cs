using Vigo360.InfobusBot.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Vigo360.InfobusBot;

public class TelegramWorker(IConfiguration configuration, ILogger<TelegramWorker> logger, BuscarCommandHandler bch,
        ParadaCommandHandler pch, InformationCommandHandler ich, DefaultCommandHandler dch)
    : BackgroundService
{
    private readonly string _token = configuration["Token"] ?? string.Empty;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        TelegramBotClient client = new(_token);
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = new[]
            {
                UpdateType.Message
            },
            ThrowPendingUpdates = true
        };

        client.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cancellationToken
        );

        var me = await client.GetMeAsync(cancellationToken: cancellationToken);

        Console.WriteLine($"Listening for @{me.Username}");
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message is not { } message)
            return;

        if (message.Location is not null)
        {
            await bch.Handle(message, botClient);
            return;
        }

        if (message.Text is not { } messageText)
            return;

        logger.LogInformation(
            Events.MessageReceived,
            "{User}: {MessageText}",
            message.From?.Username, message.Text
        );

        var args = messageText.Split(' ');

        ICommandHandler? handler = args[0][1..] switch
        {
            "start" => ich,
            "help" => ich,
            "info" => ich,
            "parada" => pch,
            "buscar" => bch,
            _ => null
        };

        if (handler is null)
        {
            if (int.TryParse(messageText, out _))
            {
                handler = pch;
            }
            else
            {
                handler = dch;
            }
        }

        try
        {
            await handler.Handle(message, botClient);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Excepción no controlada");
            await botClient.SendTextMessageAsync(chatId: message.Chat.Id, replyToMessageId: message.MessageId,
                text: "Error inesperado en el sistema", parseMode: ParseMode.Html
                , cancellationToken: cancellationToken);
        }
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "API error");

        return Task.CompletedTask;
    }
}