using System.Text;
using HtmlAgilityPack;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotVitrasa.Handlers;

public sealed class ParadaCommandHandler : ICommandHandler
{
    private static SocketsHttpHandler _handler = new()
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(15)
    };

    public async Task Handle(Message message, ITelegramBotClient client)
    {
        var args = message.Text!.Split(' ');

        if (args.Length > 2)
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                replyToMessageId: message.MessageId,
                text: "Debes especificar solo una parada",
                parseMode: ParseMode.Html
            );
        
        string id = args.Length switch
        {
            1 => args[0],
            2 => args[1],
            _ => throw new ArgumentOutOfRangeException()
        };

        if (!int.TryParse(id, out _))
        {
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                replyToMessageId: message.MessageId,
                text: "El id de la parada debe ser un número válido",
                parseMode: ParseMode.Html
            );
            return;
        }

        var paradaSolicitada = await GetParada(id);

        if (paradaSolicitada is null)
        {
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                replyToMessageId: message.MessageId,
                text: "No se ha encontrado la parada o ningún paso próximo",
                parseMode: ParseMode.Html
            );
            return;
        }

        var sb = new StringBuilder();

        sb.AppendLine($"<b>{paradaSolicitada.Nombre}</b> ({paradaSolicitada.Id})");

        sb.AppendLine($"<pre>Min | Liña => Destino</pre>");
        foreach (var paso in paradaSolicitada.Pasos)
        {
            var minutosPadded = paso.Minutos.PadLeft(3, ' ');
            var lineaPadded = paso.Linea.PadLeft(4, ' ');

            sb.AppendLine($"<pre>{minutosPadded} | {lineaPadded} => {paso.Destino}</pre>");
        }

        await client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            replyToMessageId: message.MessageId,
            text: sb.ToString(),
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardMarkup(
                new KeyboardButton("/parada " + paradaSolicitada.Id)
            )
        );
    }

    private static async Task<Parada?> GetParada(string idParada)
    {
        HttpClient cli = new(_handler)
        {
            BaseAddress = new Uri("http://infobus.vitrasa.es:8002")
        };

        var response = await cli.GetAsync($"/Default.aspx?parada={idParada}");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error: {response.StatusCode}");
            return null;
        }

        var html = await response.Content.ReadAsStringAsync();

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var nombre = doc.DocumentNode.SelectSingleNode("//*[@id=\"lblNombre\"]");
        var pasos = GetProximosPasos(doc);

        return new Parada(
            idParada,
            nombre.InnerText,
            pasos.ToArray()
        );
    }

    private static List<Paso> GetProximosPasos(HtmlDocument doc)
    {
        var rows = doc.DocumentNode.SelectNodes("//*[@id=\"GridView1\"]/tr");

        if (rows is null)
        {
            return new List<Paso>();
        }

        var pasos = new List<Paso>();

        var first = true;
        foreach (var row in rows)
        {
            if (first)
            {
                first = false;
                continue;
            }

            var cells = row.SelectNodes("td");

            if (cells is null || cells.Count != 3)
            {
                continue;
            }

            var linea = cells[0].InnerText;
            var destino = cells[1].InnerText;
            var minutos = cells[2].InnerText;

            pasos.Add(new Paso(linea, destino, minutos));
        }

        return pasos;
    }
}

record Parada(string Id, string Nombre, Paso[] Pasos);

record Paso(string Linea, string Destino, string Minutos);