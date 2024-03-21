using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMCBot.Services;

namespace UMCBot
{
    public class UMCBot
    {
        public DiscordSocketClient DiscordClient { get; private set; } = null!;

        private readonly List<IService> _services = new();
        public IEnumerable<IService> Services => _services.AsReadOnly();

        public async Task Initialize()
        {
            // GuildBans is currently unused, but I don't want to forget about it later.
            var discordConfig = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildBans | GatewayIntents.GuildMessages | GatewayIntents.GuildMembers | GatewayIntents.MessageContent
            };

            DiscordClient = new DiscordSocketClient(discordConfig);

            DiscordClient.Log += OnLog;

            // Initialize the services used by the bot.
            // TODO: Make it so a guild can disable/enable these as it needs.

            await DiscordClient.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("BOT_TOKEN"));

            await DiscordClient.StartAsync();

            await DiscordClient.SetGameAsync("UMC Bot");

            // Keep current Task alive to prevent program from closing.
            await Task.Delay(-1);
        }

        // TODO: Expand into proper console/file logging.
        private static async Task OnLog(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            await Task.CompletedTask;
        }
    }
}
