using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotVitrasa.Handlers;

public class InfoCommandHandler : ICommandHandler
{
    public async Task Handle(Message message, ITelegramBotClient client)
    {
        await client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            replyToMessageId: message.MessageId,
            text:
"""
Infobus Vigo es un bot de Telegram que te permite consultar el tiempo de espera de los autobuses de Vitrasa en Vigo.

Usa /help para ver los comandos disponibles.

Código fuente: https://github.com/arielcostas/infobus-bot

Desarrollado por Ariel Costas <https://costas.dev>
""",
            parseMode: ParseMode.Html
        );

    }
}