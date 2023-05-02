namespace BotVitrasa.Handlers;

public class DefaultCommandHandler : ICommandHandler
{
    public Task<string> Handle(string[] args)
    {
        return Task.FromResult<string>("Comando no reconocido.");
    }
}