using Discord_Bot.src.Modules.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using ThreeFourteen.Finnhub.Client;

namespace DiscordStockBot
{
    public class Bot
    {

        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public static string prefix;
        public static string finnKey;

        public async Task RunAsync()
        {
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<Configjson>(json);

            var botkey = Environment.GetEnvironmentVariable("DISCORD_STOCK_BOT_TOKEN");
            if (string.IsNullOrEmpty(botkey)) { 
                throw new Exception("Environment variable DISCORD_STOCK_BOT_TOKEN is not set!");
            }

            finnKey = Environment.GetEnvironmentVariable("DISCORD_STOCK_BOT_FINNHUB_API_KEY");
            if (string.IsNullOrEmpty(botkey))
            {
                throw new Exception("Environment variable DISCORD_STOCK_FINNHUB_BOT_API_KEY is not set!");
            }


            var config = new DiscordConfiguration
            {
                Token = botkey,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };

            Client = new DiscordClient(config);

            if (configJson.Prefix.Length > 0)
            {
                prefix = configJson.Prefix[0];
            }

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = configJson.Prefix,
                EnableDms = true,
                EnableMentionPrefix = true,
                EnableDefaultHelp = false,
                IgnoreExtraArguments = true,
            };
            Commands = Client.UseCommandsNext(commandsConfig);

            //REGRISTRATION FOR COMMAND FILES:
            Commands.RegisterCommands<Commands>();

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }
    }
}
