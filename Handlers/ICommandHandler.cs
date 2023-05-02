namespace BotVitrasa;

public interface ICommandHandler
{
    Task<string> Handle(string[] args);
}