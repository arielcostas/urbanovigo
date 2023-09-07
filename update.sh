#!/usr/bin/env bash

git pull

dotnet publish --output /opt/bot src/BotVitrasa.csproj --self-contained true -p Release

systemctl restart infobus-bot.service