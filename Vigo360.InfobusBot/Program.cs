using Vigo360.InfobusBot;
using Vigo360.InfobusBot.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets(typeof(Program).Assembly, optional: true)
    .AddEnvironmentVariables("INFOBUSBOT_");

if (builder.Environment.IsProduction())
{
    builder.Logging.AddSystemdConsole();
}

Console.WriteLine($"Read token from environment: {Environment.GetEnvironmentVariable("Token")}");

var httpClient = new HttpClient()
{
    BaseAddress = new Uri("http://infobus.vitrasa.es:8002")
};

builder.Services.AddSingleton(httpClient);

builder.Services.AddSingleton<BuscarCommandHandler>();
builder.Services.AddSingleton<DefaultCommandHandler>();
builder.Services.AddSingleton<InformationCommandHandler>();
builder.Services.AddSingleton<ParadaCommandHandler>();

builder.Services.AddSingleton(() => builder.Configuration);
builder.Services.AddHostedService<TelegramWorker>();

var app = builder.Build();

app.Run();