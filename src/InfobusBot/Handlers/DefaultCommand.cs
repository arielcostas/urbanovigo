using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Costasdev.VigoTransitTelegramBot.Handlers;

public class DefaultCommand : ICommand
{
    public async Task Handle(Message message, ITelegramBotClient client)
    {
        await client.SendMessage(
            chatId: message.Chat.Id,
            replyParameters: message.MessageId,
            text: "Comando no reconocido.",
            parseMode: ParseMode.Html
        );
        
    }
}