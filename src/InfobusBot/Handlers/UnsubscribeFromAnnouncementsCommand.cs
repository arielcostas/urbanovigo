using Telegram.Bot;
using Telegram.Bot.Types;
using Vigo360.VitrApi.TelegramBot.Handlers;

namespace Vigo360.VitrApi.TelegramBot.Handlers;

public class UnsubscribeFromAnnouncementsCommand(AppDbContext db) : ICommand
{
    public async Task Handle(Message message, ITelegramBotClient client)
    {
        var chatId = message.Chat.Id;
        
        var subscriber = await db.Subscribers.FindAsync(chatId);
        if (subscriber is null)
        {
            // TODO: Handle this case
            throw new InvalidOperationException("No se ha encontrado el suscriptor");
        }
        
        db.Subscribers.Remove(subscriber);
        await db.SaveChangesAsync();
    }
}