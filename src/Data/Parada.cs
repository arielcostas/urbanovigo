namespace BotVitrasa.Data;

record Parada(string Id, string Nombre, Paso[] Pasos);

record Paso(string Linea, string Destino, string Minutos);