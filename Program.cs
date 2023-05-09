using BotVitrasa;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<TelegramWorker>(_ => new TelegramWorker(hostContext.Configuration["Token"]!));
    });

builder.Build().Run();




