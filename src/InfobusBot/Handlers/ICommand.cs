using Telegram.Bot;
using Telegram.Bot.Types;

namespace Costasdev.VigoTransitTelegramBot.Handlers;

public interface ICommand
{
    Task Handle(Message message, ITelegramBotClient client);
}