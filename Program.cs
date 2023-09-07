using DSharpPlus;

namespace Abyss_Bot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var bot = new Bot();
            bot.Init();
            await bot.RunAsync();
            //Console.WriteLine
        }
    }

}