using Telegram.Bot;
using Telegram.Bot.Types;

namespace Vigo360.InfobusBot.Handlers;

public interface ICommandHandler
{
    Task Handle(Message message, ITelegramBotClient client);
}