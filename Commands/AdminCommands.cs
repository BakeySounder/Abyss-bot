using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace Abyss_Bot.Commands
{
    public class AdminCommands : ApplicationCommandModule
    {
        [SlashRequirePermissions(Permissions.Administrator)]
        [SlashCommand("Init", "Заносит сервер в БД")]
        public async Task Init(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            var result = await Bot.database.InitGuild(ctx.Channel);
            if (result)
            {

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Выполнено"));
                Bot.exps = await Bot.database.GetExp();
            }
            else await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Произошла ошибка, сервер уже есть в БД"));
        }

        [SlashRequirePermissions(Permissions.BanMembers)]
        [SlashCommand("Ban", "Банит пользователя")]
        public async Task Ban(InteractionContext ctx,
            [Option("Пользователь", "Кого банить")] DiscordUser user,
            [Option("Причина", "Причина бана")] string reason,
            [Choice("Нисколько", 0)]
            [Choice("1 День", 1)]
            [Choice("1 Неделя", 7)]
            [Option("Время", "Кол-во дней для очистки сообщений")] long deleteDays = 0)
        {
            var member = await ctx.Guild.GetMemberAsync(user.Id);

            var result = await Bot.database.GettextchannelId(ctx.Channel.GuildId);
            if (result != 0)
            {
                var embed = new DiscordEmbedBuilder
                {
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl, Height = 30, Width = 30 }
                };
                embed.WithColor(DiscordColor.Cyan);
                embed.WithTitle($"Был забанен пользователь");
                embed.AddField("Пользователь:", $"{member.Mention}");
                embed.AddField("Кто забанил:", $"{ctx.Member.Mention}");
                embed.AddField("Причина:", $"{reason}");
                embed.WithTimestamp(DateTime.Now);
                embed.WithFooter("© Abyss Bot 2023");

                var channel = ctx.Channel.Guild.GetChannel(result);
                await channel.SendMessageAsync(embed.Build());
            }
            var result2 = Bot.database.LogBan(ctx.Channel, member, ctx.Member, reason);
            await ctx.Guild.BanMemberAsync(user.Id, (int)deleteDays,reason);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Пользователь {user.Username} был забанен"));
        }


        [SlashRequirePermissions(Permissions.MuteMembers)]
        [SlashCommand("Mute", "Заглушает пользователя")]
        public async Task Mute(InteractionContext ctx,
            [Option("Пользователь", "Кого заглушить")] DiscordUser user,
            [Choice("5 Минут", 5)]
            [Choice("10 Минут", 10)]
            [Choice("30 Минут", 30)]
            [Choice("1 Час", 60)]
            [Choice("3 Часа", 180)]
            [Choice("1 День", 1440)]
            [Choice("3 Дня", 4320)]
            [Option("Время", "На сколько заглушить")] long timeMinutes,
            [Option("Причина", "Причина заглушения")] string reason)
        {
            var member = await ctx.Guild.GetMemberAsync(user.Id);
            var stringTime = "";
            switch (timeMinutes)
            {
                case 5:
                    stringTime = "5 Минут";
                    break;
                case 10:
                    stringTime = "10 Минут";
                    break;
                case 30:
                    stringTime = "30 Минут";
                    break;
                case 60:
                    stringTime = "1 Час";
                    break;
                case 180:
                    stringTime = "3 Часа";
                    break;
                case 1440:
                    stringTime = "1 День";
                    break;
                case 4320:
                    stringTime = "3 Дня";
                    break;
            }
            var result = await Bot.database.GettextchannelId(ctx.Channel.GuildId);
            if (result != 0)
            {
                var embed = new DiscordEmbedBuilder
                {
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl, Height = 30, Width = 30 }
                };
                embed.WithColor(DiscordColor.Goldenrod);
                embed.WithTitle($"Был заглушен пользователь");
                embed.AddField("Пользователь:", $"{member.Mention}");
                embed.AddField("Кто заглушил:", $"{ctx.Member.Mention}");
                embed.AddField("Причина:", $"{reason}");
                embed.AddField("На период:", $"{stringTime}");
                embed.WithTimestamp(DateTime.Now);
                embed.WithFooter("© Abyss Bot 2023");

                var channel = ctx.Channel.Guild.GetChannel(result);
                await channel.SendMessageAsync(embed.Build());
            }
            var result2 = Bot.database.LogMute(ctx.Channel, member, ctx.Member, reason, timeMinutes);
            await member.TimeoutAsync(DateTimeOffset.Now.AddMinutes(timeMinutes), reason);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Пользователь {user.Username} был заглушен, на период {stringTime}"));
        }

        [SlashRequirePermissions(Permissions.KickMembers)]
        [SlashCommand("Kick", "Выгоняет пользователя")]
        public async Task Kick(InteractionContext ctx,
            [Option("user", "Кого кикать")] DiscordUser user,
            [Option("Причина", "Причина кика")] string reason)
        {
            var member = await ctx.Guild.GetMemberAsync(user.Id);

            var result = await Bot.database.GettextchannelId(ctx.Channel.GuildId);
            if (result != 0)
            {

                var embed = new DiscordEmbedBuilder
                {
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl, Height = 30, Width = 30 }
                };
                embed.WithColor(DiscordColor.Sienna);
                embed.WithTitle($"Был выгнан пользователь");
                embed.AddField("Пользователь:", $"{member.Mention}");
                embed.AddField("Кто выгнал:", $"{ctx.Member.Mention}");
                embed.AddField("Причина:", $"{reason}");
                embed.WithTimestamp(DateTime.Now);
                embed.WithFooter("© Abyss Bot 2023");

                var channel = ctx.Channel.Guild.GetChannel(result);
                await channel.SendMessageAsync(embed.Build());
            }
            var result2 = Bot.database.LogKick(ctx.Channel, member, ctx.Member, reason);
            await member.RemoveAsync(reason);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Пользователь {user.Username} был выгнан"));
        }

        [SlashRequirePermissions(Permissions.ManageMessages)]
        [SlashCommand("Clear", "Удаляет сообщения")]
        public async Task Clear(InteractionContext ctx,
            [Choice("5 сообщений", 5)]
            [Choice("10 сообщений", 10)]
            [Choice("30 сообщений", 30)]
            [Choice("60 сообщений", 60)]
            [Option("Количество", "Кол-во сообщений")] long messageCount)
        {
            var last_msg = (await ctx.Channel.GetMessagesAsync((int)messageCount + 1)).ToList();
            last_msg.Reverse();
            for (int i = 0; i < last_msg.Count; i++)
            {
                await last_msg[i].DeleteAsync();
            }

            var result = await Bot.database.GettextchannelId(ctx.Channel.GuildId);
            if (result != 0)
            {


                var embed = new DiscordEmbedBuilder
                {
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = ctx.Member.AvatarUrl, Height = 30, Width = 30 }
                };
                embed.WithColor(DiscordColor.Gold);
                embed.WithTitle($"Были удалены сообщения");
                embed.AddField("Администратор:", $"{ctx.Member.Mention}");
                embed.AddField("Кол-во:", $"{last_msg.Count} сообщений(я)");
                embed.AddField("Канал:", $"{ctx.Channel.Mention}");
                embed.WithTimestamp(DateTime.Now);
                embed.WithFooter("© Abyss Bot 2023");

                var channel = ctx.Channel.Guild.GetChannel(result);
                await channel.SendMessageAsync(embed.Build());
            }
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Было удалено {last_msg.Count} сообщений(я)"));
        }

        [SlashCommand("Banlist", "Получает информацию о забаненных")]
        [SlashRequirePermissions(Permissions.BanMembers)]
        public async Task BanList(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            var embed = new DiscordEmbedBuilder
            {
                
            };
            
            var banned = "";
            var ban_list = await ctx.Guild.GetBansAsync();
            foreach (var item in ban_list)
            {
                var reason = item.Reason == null ? "нет причины" : item.Reason;
                banned += item.User.Id + " | " + item.User.Username + " | " + reason + '\n';
            }
            embed.WithTitle("Список забаненных");
            embed.WithColor(DiscordColor.Lilac);
            embed.AddField("UserID | UserName | Reason", banned == "" ? "NULL":banned);
            embed.WithFooter("© Abyss Bot 2023");
            //embed.AddField("Тип сервера:", $"{serverData.guildType}");

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()));
            //await ctx.Channel.SendMessageAsync(embed.Build());
        }

        [SlashCommand("Unban", "Снимает бан с пользователя")]
        [SlashRequirePermissions(Permissions.BanMembers)]
        public async Task Unban(InteractionContext ctx, 
            [Option("ID_пользователя", "UserID пользователя")] string UserID, 
            [Option("Причина", "Причина")] string reason)
        {
            ulong user_id;
            if (!ulong.TryParse(UserID, out user_id)) {
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("Произошла ошибка"));
                return;
            };

            var ban_list = (await ctx.Guild.GetBansAsync()).ToList<DiscordBan>();   
            var target = ban_list.Find(y => y.User.Id == ulong.Parse(UserID));
            if(target== null)
            {
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("Произошла ошибка"));
                return;
            }

            var user = target.User;
            //embed.AddField("Тип сервера:", $"{serverData.guildType}");
            await ctx.Guild.UnbanMemberAsync(user);

            var result = await Bot.database.GettextchannelId(ctx.Channel.GuildId);
            if (result != 0)
            {
                var embed = new DiscordEmbedBuilder
                {
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = ctx.Member.AvatarUrl, Height = 30, Width = 30 }
                };
                embed.WithColor(DiscordColor.SpringGreen);
                embed.WithTitle($"Были разбанен пользователь");
                embed.AddField("Администратор:", $"{ctx.Member.Mention}");
                embed.AddField("Кто разбанен:", $"{user.Mention}");
                embed.AddField("Причина:", $"{reason}");
                embed.WithTimestamp(DateTime.Now);
                embed.WithFooter("© Abyss Bot 2023");

                var channel = ctx.Channel.Guild.GetChannel(result);
                await channel.SendMessageAsync(embed.Build());
            }
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("Выполнено"));
            //await ctx.Channel.SendMessageAsync(embed.Build());
        }
    }
}
