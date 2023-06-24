using Microsoft.Extensions.Logging;

namespace BotVitrasa;

public class Events
{
    public static readonly EventId BotStarted = new(1, "Bot started");
    public static readonly EventId MessageReceived = new(2, "Message received");
    public static readonly EventId BotError = new(3, "Bot error");

}