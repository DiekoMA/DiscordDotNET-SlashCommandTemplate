namespace DiscordDotNETTemplate.utils;

public class InteractionHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _commands;
    private readonly IServiceProvider _services;
    IUserMessage answerMessage;

    public InteractionHandler(DiscordSocketClient client, InteractionService commands, IServiceProvider services)
    {
        _client = client;
        _commands = commands;
        _services = services;
    }

    public async Task InitializeAsync()
    {
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        _client.InteractionCreated += HandleInteraction;
        _client.Connected += HandleOnConnected;
    }

    private async Task HandleOnConnected()
    {
        //Set the rich presence of the bot.
        await _client.SetGameAsync("Hello World", "", ActivityType.Playing);
    }

    private async Task HandleInteraction(SocketInteraction args)
    {
        try
        {
            var ctx = new SocketInteractionContext(_client, args);
            await _commands.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
