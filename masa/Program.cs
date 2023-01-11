using Discord;
using Discord.WebSocket;
using System.Net.Http.Json;

public class Program
{
    private DiscordSocketClient? _client;
    private string? _token;
    public static Task Main(string[] args) => new Program().MainAsync(args);

    public async Task MainAsync(string[] args) { 
        _client = new DiscordSocketClient();
        _client.Log += Log;
        _token = args[0];
        
        await _client.LoginAsync(TokenType.Bot, _token);
        await _client.StartAsync();
        await SetInterval(() => LaitaKissakuva(), TimeSpan.FromHours(12),true);

        await Task.Delay(-1);
    }
    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    private async void LaitaKissakuva()
    {
   
        var guild = _client?.GetGuild(252154029868711937);
        var channel = guild?.GetTextChannel(695761833004367973);
        HttpClient httpClient = new HttpClient();
        var data = await httpClient.GetFromJsonAsync<List<Kissakuva>>("https://api.thecatapi.com/v1/images/search");

        if (data?[0] != null && channel != null)
        {
            await channel.SendMessageAsync(data[0].url);
        }

    }
    public static async Task SetInterval(Action action, TimeSpan timeout, bool firstTime)
    {
        if (firstTime)
        {
            //Botti lähettää käynnistymisessä ekan kuvan ja odottaa 10 sekuntia että on kerinnyt kirjautumaan.
            await Task.Delay(10000).ConfigureAwait(false);
            action();
        }

        await Task.Delay(timeout).ConfigureAwait(false);
   
        action();
   
        await SetInterval(action, timeout,false);
    }
    public class Kissakuva
    {
        public object[]? breeds { get; set; }
        public string? id { get; set; }
        public string? url { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
    }
}