using Abyss_Bot.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace Abyss_Bot
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }

        public CommandsNextExtension Commands { get; private set; }
        public SlashCommandsExtension SlashCommands { get; private set; }

        public static Database database;

        async public void Init()
        {
            database = new Database();
            await database.Connect();
            await database.CreateTablesIfNotExists();
        }

        public static Dictionary<ulong, Dictionary<ulong, int>> exps;
        public static Dictionary<ulong, Dictionary<ulong, bool>> Update;

        public async Task RunAsync()
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("Config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
            {
                json = await sr.ReadToEndAsync().ConfigureAwait(false);
            }
            var configJson = JsonConvert.DeserializeObject<Config>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
                Intents = DiscordIntents.All
            };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;
            Client.MessageCreated += OnMessageCreated;
            

            var slash = Client.UseSlashCommands();

            slash.RegisterCommands<AdminCommands>();
            slash.RegisterCommands<UserCommands>();
            

            await Client.ConnectAsync();


            
            exps = await database.GetExp();
            Update = new Dictionary<ulong, Dictionary<ulong, bool>>();
            foreach (var item in exps)
            {
                Update[item.Key] = new Dictionary<ulong, bool>();
                foreach (var item2 in item.Value)
                {
                    Update[item.Key][item2.Key] = false;
                }
            }
            var stateTimer = new Timer(CheckExp, new AutoResetEvent(false), 0, 5000);
            await Task.Delay(-1);


        }
        async public void CheckExp(Object stateInfo)
        {

            foreach (var item in exps)
            {
                foreach (var item2 in item.Value)
                {
                    if (Update[item.Key][item2.Key])
                    {
                        database.UpdateExp(item.Key, item2.Key, item2.Value);
                        Update[item.Key][item2.Key] = false;
                    }
                }
            }

        }

        private async Task OnMessageCreated(DiscordClient sender, MessageCreateEventArgs args)
        {
            if (args.Author.IsBot) return;
            if (exps.ContainsKey((ulong)args.Channel.GuildId) && !exps[(ulong)args.Channel.GuildId].ContainsKey(args.Author.Id))
            {
                await database.InsertUser(args.Channel,args.Author);
                exps = await database.GetExp();
                Update = new Dictionary<ulong, Dictionary<ulong, bool>>();
                foreach (var item in exps)
                {
                    Update[item.Key] = new Dictionary<ulong, bool>();
                    foreach (var item2 in item.Value)
                    {
                        Update[item.Key][item2.Key] = false;
                    }
                }
            }
            else
            {
                exps[(ulong)args.Channel.GuildId][args.Author.Id] +=5;
                Update[(ulong)args.Channel.GuildId][args.Author.Id] = true;
                if (exps[(ulong)args.Channel.GuildId][args.Author.Id] >= 100)
                {
                    exps[(ulong)args.Channel.GuildId][args.Author.Id] = 0;
                    await database.UpdateLevel(args.Channel,args.Author); // ПОЛУЧЕНИЕ УРОВНЯ
                }
            }
            Console.WriteLine(args.Message);
            return ;
            
        }

        private Task OnClientReady(DiscordClient sender, ReadyEventArgs args)
        {
            
            return Task.CompletedTask;
        }
    }
}
