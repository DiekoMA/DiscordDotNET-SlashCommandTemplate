namespace DiscordDotNETTemplate.modules;

public class module1 : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Responds with pong!")]
    public async Task PingCommand()
    {
        await Context.Interaction.RespondAsync("Pong!");
    }
}
