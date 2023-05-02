using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotVitrasa.Handlers;

public interface ICommandHandler
{
    Task Handle(Message message, ITelegramBotClient client);
}