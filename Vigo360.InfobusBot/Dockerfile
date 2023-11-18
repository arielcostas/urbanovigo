FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Vigo360.InfobusBot.csproj", "./"]
RUN dotnet restore "Vigo360.InfobusBot.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Vigo360.InfobusBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Vigo360.InfobusBot.csproj" -c Release -o /app/publish

FROM base AS final
LABEL org.opencontainers.image.source = "https://github.com/arielcostas/infobus-bot"
LABEL org.opencontainers.image.authors = "Ariel Costas <ariel@costas.dev>"

WORKDIR /app
COPY --from=publish /app/publish .
ENV INFOBUSBOT_Token ""
ENTRYPOINT ["dotnet", "BotVitrasa.dll"]
