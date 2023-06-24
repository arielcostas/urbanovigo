using BotVitrasa;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<TelegramWorker>();

var app = builder.Build();

app.Run();