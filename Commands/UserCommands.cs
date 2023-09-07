using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Abyss_Bot.Commands
{
    internal class UserCommands : ApplicationCommandModule

    {

        [SlashCommand("ServerInfo", "Получает информацию о сервере")]
        public async Task ServerInfo(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            var serverData = await Bot.database.GetServerData(ctx.Channel.GuildId);
            if (serverData != null)
            {
                var embed = new DiscordEmbedBuilder
                {
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = ctx.Channel.Guild.IconUrl, Height = 100, Width = 100 }
                };
                embed.WithColor(DiscordColor.CornflowerBlue);
                embed.WithTitle($"Информация о сервере");
                embed.AddField("Владелец сервера:", $"{Formatter.Mention(ctx.Channel.Guild.Owner)}");
                embed.AddField("Дата создания:", $"{ctx.Channel.Guild.CreationTimestamp.DateTime}");
                embed.AddField("Количество участников:", $"{ctx.Channel.Guild.MemberCount}");
                embed.AddField("Дата инициализации:", $"{serverData.InitDate}");
                //embed.AddField("Тип сервера:", $"{serverData.guildType}");

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()));
                //await ctx.Channel.SendMessageAsync(embed.Build());
            }
            else await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Произошла ошибка"));
        }

        [SlashCommand("User", "Получает информацию о вас")]
        public async Task User(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            var userData = await Bot.database.GetUserData(ctx.Channel.GuildId,ctx.Member.Id);
            if (userData != null)
            {
                var embed = new DiscordEmbedBuilder
                {
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = ctx.Member.AvatarUrl, Height = 100, Width = 100 }
                };
                var roles = "";
                foreach (var item in ctx.Member.Roles)
                {
                    roles += item.Mention + " ";
                }
                embed.WithColor(DiscordColor.Lilac);
                embed.WithTitle($"Информация о пользователе");
                embed.AddField("Никнейм:", $"{ctx.Member.DisplayName}");
                embed.AddField("Роли:", $"{roles}");
                embed.AddField("В дискорде с:", $"{ctx.Member.CreationTimestamp.DateTime}");
                embed.AddField("На сервере с:", $"{ctx.Member.JoinedAt.DateTime}");
                embed.AddField("Количество опыта:", $"{userData.exp} exp / 100 exp");
                embed.AddField("Уровень:", $"{userData.lvl} lvl");
                //embed.AddField("Тип сервера:", $"{serverData.guildType}");

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()));
                //await ctx.Channel.SendMessageAsync(embed.Build());
            }
            else await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Произошла ошибка"));
        }
    }
}
