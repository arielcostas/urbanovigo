#!/bin/sh

git pull

sudo systemctl stop infobus-bot.service

dotnet publish --output /opt/bot src/BotVitrasa.csproj --self-contained true -p Release

sudo systemctl start infobus-bot.service
