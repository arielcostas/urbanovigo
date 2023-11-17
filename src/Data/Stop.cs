﻿namespace BotVitrasa.Data;

internal record Stop(string Id, string Name, Arrival[] Arrivals);

internal record Arrival(string Line, string Headsign, string EstimatedMinutes);