using Costasdev.VigoTransitApi;
using Costasdev.VigoTransitTelegramBot;
using Costasdev.VigoTransitTelegramBot.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<HttpClient>();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets(typeof(Program).Assembly, optional: true)
    .AddEnvironmentVariables("INFOBUSBOT_");

if (builder.Environment.IsProduction())
{
    builder.Logging.AddSystemdConsole();
}

builder.Services.AddSingleton<VigoTransitApiClient>();
builder.Services.AddSingleton<SearchStopsCommand>();
builder.Services.AddSingleton<DefaultCommand>();
builder.Services.AddSingleton<InformationCommand>();
builder.Services.AddSingleton<FindStopCommand>();

builder.Services.AddSingleton(() => builder.Configuration);
builder.Services.AddHostedService<TelegramWorker>();

var app = builder.Build();

app.Run();