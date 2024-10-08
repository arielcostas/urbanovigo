﻿using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Costasdev.VigoTransitTelegramBot.Handlers;

public class DefaultCommand : ICommand
{
    public async Task Handle(Message message, ITelegramBotClient client)
    {
        await client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            replyToMessageId: message.MessageId,
            text: "Comando no reconocido.",
            parseMode: ParseMode.Html
        );
        
    }
}