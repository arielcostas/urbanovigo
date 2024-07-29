using Telegram.Bot;
using Telegram.Bot.Types;

namespace Vigo360.VitrApi.TelegramBot.Handlers;

public interface ICommand
{
    Task Handle(Message message, ITelegramBotClient client);
}