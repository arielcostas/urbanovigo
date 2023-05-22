using BotVitrasa.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<TelegramWorker> _logger;

    public TelegramWorker(IConfiguration configuration, ILogger<TelegramWorker> logger)
    {
        _token = configuration["Token"]!;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        TelegramBotClient client = new(_token);
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = new []
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

        if (message.Text is not { } messageText)
            return;

        _logger.LogInformation($"{message.Chat.Username}: {messageText}");
        
        var args = messageText.Split(' ');

        ICommandHandler? handler = args[0][1..] switch
        {
            "start" => new StartCommandHandler(),
            "help" => new HelpCommandHandler(),
            "info" => new InfoCommandHandler(),
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
                => $"Telegram API Error: [{apiRequestException.ErrorCode}] {apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}