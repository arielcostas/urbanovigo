using BotVitrasa.Handlers;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotVitrasa;

public class TelegramWorker : BackgroundService
{
    private readonly string _token;
    private readonly BuscarCommandHandler _bch = new();

    public TelegramWorker(string token)
    {
        _token = token;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        TelegramBotClient client = new(_token);
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = new []
            {
                UpdateType.Message
            }
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
            await _bch.Handle(message, botClient);
            return;
        }
        
        if (message.Text is not { } messageText)
            return;

        var args = messageText.Split(' ');

        ICommandHandler? handler = args[0][1..] switch
        {
            "start" => new InformationCommandHandler(),
            "help" => new InformationCommandHandler(),
            "info" => new InformationCommandHandler(),
            "parada" => new ParadaCommandHandler(),
            "buscar" => _bch,
            _ => null
        };

        if (handler is null)
        {
            if (int.TryParse(messageText, out _))
            {
                handler = new ParadaCommandHandler();
            }
            else
            {
                handler = new DefaultCommandHandler();
            }
        }

        await handler.Handle(message, botClient);
    }

    private static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}