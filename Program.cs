using BotVitrasa;
using BotVitrasa.Handlers;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

TelegramBotClient client = new(configuration["Token"]!);

using CancellationTokenSource cts = new();

ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

client.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await client.GetMeAsync();

Console.WriteLine($"Listening for @{me.Username}");
Console.ReadLine();

cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Message is not { } message)
        return;
    
    if (message.Text is not { } messageText)
        return;

    var args = messageText.Split(' ');

    ICommandHandler handler = args[0][1..] switch
    {
        "parada" => new ParadaCommandHandler(),
        _ => new DefaultCommandHandler()
    };

    var reply = handler.Handle(args[1..]);

    await botClient.SendTextMessageAsync(
        chatId: message.Chat.Id,
        replyToMessageId: message.MessageId,
        text: await reply,
        parseMode: ParseMode.Html,
        cancellationToken: cancellationToken
    );
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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