﻿using BotVitrasa;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddEnvironmentVariables();

Console.WriteLine($"""Read token from environment: ${Environment.GetEnvironmentVariable("Token")}""");

builder.Services.AddSingleton(() => builder.Configuration);
builder.Services.AddHostedService<TelegramWorker>();

var app = builder.Build();

app.Run();