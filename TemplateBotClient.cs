namespace DiscordDotNETTemplate;

public class TemplateBotClient
{
    private static ServiceProvider ConfigureServices()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddYamlFile("config.yml")
            //.AddJsonFile("config.json") Incase you want to use a json file instead
            .Build();

        return new ServiceCollection()
            .AddSingleton(config)
            .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged,
                AlwaysDownloadUsers = true
            }))

            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>()
            .BuildServiceProvider();
    }

    public async Task RunAsync()
    {
        await using var services = ConfigureServices();
        var config = services.GetRequiredService<IConfigurationRoot>();
        var _client = services.GetRequiredService<DiscordSocketClient>();
        var sCommands = services.GetRequiredService<InteractionService>();

        await services.GetRequiredService<InteractionHandler>().InitializeAsync();

        _client.Log += async (LogMessage msg) => { Console.WriteLine(msg.Message); };
        sCommands.Log += async (LogMessage msg) => { Console.WriteLine(msg.Message); };

        _client.Ready += async () =>
        {
            //await sCommands.RegisterCommandsToGuildAsync(ulong.Parse(config["GuildID"])); //Use this if you want commands to only run on your private server
            await sCommands.RegisterCommandsGloballyAsync();

            //This is just for style, it prints out a table of all commands and their coresponding information.
            var commandTable = new Table();
            commandTable.Title("Commands");
            commandTable.AddColumn("Command Name");
            commandTable.AddColumn(new TableColumn("Description").Centered());
            commandTable.AddColumn(new TableColumn("Module/Group").Centered());
            foreach (var slashCommand in sCommands.SlashCommands)
            {
                commandTable.AddRow(slashCommand.Name, slashCommand.Description, slashCommand.Module.Name);
            }
            AnsiConsole.Write(commandTable);
        };

        await _client.LoginAsync(TokenType.Bot, config["tokens:discord"]);
        await _client.StartAsync();

        await Task.Delay(-1);
    }
}
