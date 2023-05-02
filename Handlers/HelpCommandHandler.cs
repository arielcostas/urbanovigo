﻿using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotVitrasa.Handlers;

public class HelpCommandHandler : ICommandHandler
{
    public async Task Handle(Message message, ITelegramBotClient client)
    {
        await client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            replyToMessageId: message.MessageId,
            text:
"""
Hola 👋

Soy un bot no oficial para consultar rápidamente información de las paradas de Vitrasa.

<b>Comandos disponibles:</b>

/start - Inicia el bot
/help - Muestra esta ayuda

/buscar &lt;termino&gt; - Busca una parada
/parada &lt;id&gt; - Muestra información de una parada
""",
            parseMode: ParseMode.Html
        );

    }
}