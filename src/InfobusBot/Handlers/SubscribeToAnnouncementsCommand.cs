using Telegram.Bot;
using Telegram.Bot.Types;
using Vigo360.VitrApi.TelegramBot.Data.Models;
using Vigo360.VitrApi.TelegramBot;

namespace Vigo360.VitrApi.TelegramBot.Handlers;

public class SubscribeToAnnouncementsCommand(AppDbContext db) : ICommand
{
    public async Task Handle(Message message, ITelegramBotClient client)
    {
        var chatId = message.Chat.Id;
        
        db.Subscribers.Add(new Subscriber
        {
            ChatId = chatId.ToString(),
            SubscriptionDate = DateTime.Now
        });
        await db.SaveChangesAsync();
    }
}