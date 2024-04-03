using Microsoft.EntityFrameworkCore;
using Vigo360.VitrApi.TelegramBot.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vigo360.VitrApi.TelegramBot;

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

builder.Services.AddSingleton<SearchStopsCommand>();
builder.Services.AddSingleton<DefaultCommand>();
builder.Services.AddSingleton<InformationCommand>();
builder.Services.AddSingleton<FindStopCommand>();
builder.Services.AddSingleton<SubscribeToAnnouncementsCommand>();
builder.Services.AddSingleton<UnsubscribeFromAnnouncementsCommand>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("DefaultConnection is not configured");
    }

    options.UseMySQL(connectionString);
}, contextLifetime: ServiceLifetime.Singleton);

builder.Services.AddSingleton(() => builder.Configuration);
builder.Services.AddHostedService<TelegramWorker>();

var app = builder.Build();

app.Run();