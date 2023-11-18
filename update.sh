#!/bin/sh

git pull

sudo systemctl stop infobus-bot.service

dotnet publish --output /opt/bot Vigo360.InfobusBot/Vigo360.InfobusBot.csproj --self-contained true -p Release

sudo systemctl start infobus-bot.service