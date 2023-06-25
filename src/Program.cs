using BotVitrasa;
using BotVitrasa.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables();

if (builder.Environment.IsProduction())
{
    builder.Logging.AddSystemdConsole();
}

Console.WriteLine($"""Read token from environment: {Environment.GetEnvironmentVariable("Token")}""");

var httpClient = new HttpClient()
{
    BaseAddress = new Uri("http://infobus.vitrasa.es:8002")
};

builder.Services.AddSingleton(() => httpClient);

builder.Services.AddSingleton<BuscarCommandHandler>();
builder.Services.AddSingleton<DefaultCommandHandler>();
builder.Services.AddSingleton<InformationCommandHandler>();
builder.Services.AddSingleton<ParadaCommandHandler>();

builder.Services.AddSingleton(() => builder.Configuration);
builder.Services.AddHostedService<TelegramWorker>();

var app = builder.Build();

app.Run();