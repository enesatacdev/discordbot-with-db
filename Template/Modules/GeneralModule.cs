using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Infrastructure;
using Microsoft.Extensions.Logging;
using Template.Common;
using Template.Services;
using Template.Utilities;

namespace Template.Modules
{
    public class GeneralModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<ExampleModule> _logger;
        private readonly Images _images;
        private readonly ServerHelper _serverHelper;

        public GeneralModule(ILogger<ExampleModule> logger, Images images, ServerHelper serverHelper)
        {
            _logger = logger;
            _images = images;
            _serverHelper = serverHelper;
        }

        [Command("heythere")]
        public async Task Selam()
        {
            await Context.Channel.SendMessageAsync("Generol Kenobi :)");
        }


        [Command("sunucu")]
        [Summary("Sunucu hakkındaki bilgileri gösterir")]
        public async Task Server()
        {
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithDescription("Sunucu hakkında bilgiler!")
                .WithTitle($"{Context.Guild.Name}")
                .WithColor(new Color(33, 176, 252))
                .AddField("Kullanıcılar", (Context.Guild as SocketGuild).MemberCount, true)
                .AddField("Online Kullanıcılar", (Context.Guild as SocketGuild).Users.Where(x => x.Status != UserStatus.Offline).Count(), true)
                .AddField("Oluşturulma Tarihi", Context.Guild.CreatedAt.ToString("dd/MM/yyyy"), true);

            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }
        public static async Task PostUserInfo(SocketGuildUser user, SocketTextChannel channel)
        {
            var roles = user.Roles.Where(r => !r.Name.Contains("everyone")).ToList();
            roles.Sort((a, b) => a.CompareTo(b));

            var builder = new EmbedBuilder()
                 .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                 .AddField("Kullanıcı Adı", user.Username, true)
                 .AddField("Katılma Tarihi", user.JoinedAt.Value.ToString("dd/MM/yyyy"), true)
                 .AddField("Roller", string.Join(" ", user.Roles.Select(x => x.Mention)))
                 .WithColor(new Color(33, 176, 252))
                 .WithCurrentTimestamp();
            await channel.SendMessageAsync(embed: builder.Build());
        }

        [Command("bilgi")]
        [Summary("Kullanıcı hakkındaki bilgileri gösterir")]
        public async Task Info(SocketGuildUser usr = null)
        {
            usr = usr is null ? Context.Message.Author as SocketGuildUser : usr;
            await PostUserInfo(usr, Context.Channel as SocketTextChannel);
        }

        [Command("pp")]
        [Alias("avatar", "kullanici-resmi")]
        [Summary("Kullanıcının resmini büyük bir şekilde gösterir.")]
        public async Task Avatar(SocketGuildUser usr = null)
        {
            usr = usr is null ? Context.Message.Author as SocketGuildUser : usr;
            var e = new EmbedBuilder()
                .WithTitle("Profil Resmi")
                .WithImageUrl(usr.GetAvatarUrl(ImageFormat.Png, 2048))
                .WithColor(Color.Green)
                .WithAuthor(usr)
                .WithCurrentTimestamp();

            await ReplyAsync(embed: e.Build());
        }

        [Command("echo")]
        public async Task EchoTextAsync([Remainder] string text)
        {
            var messages = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
            string usersText = text;
            await ReplyAsync(usersText);
        }

        [Command("hatirlatici")]
        public async Task Reminder(int timer, [Remainder] string sebep = null)
        {
            var lastmessage = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(lastmessage);
            var remindermessage = await Context.Channel.SendMessageAsync($"{timer} saniye sonra sizi uyarıcam!");
            await Task.Delay(2000);
            await Context.Channel.DeleteMessageAsync(remindermessage);
            int sure = timer * 1000;
            await Task.Delay(sure);
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl())
                .WithDescription($"{timer} saniye sonra sizi uyarmamı istemiştiniz :)")
                .AddField("Kullanıcı Adı", (Context.User as SocketGuildUser).Mention.ToString(), true)
                .AddField("Hatırlatıcı Süresi", timer + " Saniye", true)
                .AddField("Uyarı!", $"Bu Mesaj Kendini Birkaç Saniye İçerisinde Yokedicek!", false)
                .WithColor(new Color(33, 176, 252))
                .WithCurrentTimestamp();


            if (sebep != null)
            {
                builder.AddField("Uyarılmak İstediğiniz Konu", sebep, false);
            }
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
            await Task.Delay(5000);
            var messages = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
        }

        [Command("bot")]
        [Alias("bot hakkında", "bot-hakkinda", "developer", "yazilimci")]
        [Summary("Bot ve developerların hakkında bilgileri içerir.")]
        public async Task DisplayBotInfoAsync()
        {
            var e = new EmbedBuilder()
                .WithAuthor(Context.Guild.Name, Context.Guild.IconUrl)
                .WithTitle("Bot Hakkında")
                .AddField("Bot'un Sahibi", "**heyimenes#2546**", true)
                .AddField("Twitter", "heyimenes", true)
                .AddField("Instagram", "heyimenes", true)
                .WithColor(Color.Purple)
                .WithFooter("Bot hakkında soru/görüş/bilgi'lerinizi heyimenes#2546'ya iletebilirsiniz.")
                .WithThumbnailUrl(Context.Message.Author.GetAvatarUrl());
            await ReplyAsync(embed: e.Build());
        }

        [Command("oneri")]
        [Alias("öneri", "oneride-bulun", "suggestion")]
        [Summary("Öneride bulun")]
        public async Task CreateSuggestionAsync(
            [Summary("Öneri.")]
                [Remainder] string suggestion)
        {
            var suggestionEmbed = new EmbedBuilder()
                .WithAuthor(Context.Message.Author)
                .WithTitle("Öneri")
                .WithColor(Discord.Color.Green)
                .WithDescription(suggestion);
            var message = await Context.Channel.SendMessageAsync(embed: suggestionEmbed.Build());
            var yes = new Emoji("✅");
            var no = new Emoji("⛔");
            await message.AddReactionAsync(yes);
            await message.AddReactionAsync(no);

        }

        [Command("davet")]
        [Summary("Get an invite to use Drudge in your own guild.")]
        public async Task GetInviteAsync()
        {
            var invite = new EmbedBuilder()
            .WithAuthor(Context.Message.Author)
            .WithTitle("Beni sunucunuza davet etmek ister misiniz?")
            .AddField("Davet Linkim Aşağıda:)", "[Buraya tıklayarak beni sunucuna davet edebilirsin.](https://discord.com/api/oauth2/authorize?client_id=816369096966275075&permissions=1609826167&scope=bot)", false)
            .AddField("Beni Desteklemek İster misin?", "Beni daha çok sunucya davet ederek destekleyebilirsin :)", false)
            .WithFooter("Beni kullandığınız için teşekkür ederim")
            .WithColor(Discord.Color.Green);
            var message = await Context.Channel.SendMessageAsync(embed: invite.Build());
        }
    }
}