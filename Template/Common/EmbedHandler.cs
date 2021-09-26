using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Template.Common
{
    public class EmbedHandler
    {
        public static async Task<Embed> NextSong(string title, string duration, string artwork)
        {
            var builder = new EmbedBuilder()
               .WithThumbnailUrl(artwork)
               .WithTitle("Sıraya Alınan Parça ")
               .AddField("Parça Adı", title, false)
               .AddField("Parça Uzunluğu", duration, false)
               .WithColor(new Color(33, 176, 252));
            var embed = builder.Build();

            return embed;
        }

        public static async Task<Embed> TranslateEmbed(string cevirilenDil,string cevirilicekDil,string cevirilicekCumle, string cevrilmisCumle)
        {
            var builder = new EmbedBuilder()
               .WithTitle($"Çeviri - {cevirilenDil}'den {cevirilicekDil}'ye")
               .AddField("Çevirilicek Cümle", cevirilicekCumle, false)
               .AddField("Çevirilmiş Cümle", cevrilmisCumle, false)
               .WithColor(new Color(33, 176, 252));
            var embed = builder.Build();

            return embed;
        }

        public static async Task<Embed> CreateBasicEmbed(string title, string description, SocketGuildUser user)
        {
            Color color = SetColor();

            var embed = await Task.Run(() => new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(color)
                .WithFooter(user.Username + "#" + user.Discriminator + " tarafından istek.", iconUrl: user.GetAvatarUrl()).Build());
            return embed;
        }

        public static async Task<Embed> CreateBasicEmbed(string title, string description)
        {
            Color color = SetColor();

            var embed = await Task.Run(() => new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(color).Build());
            return embed;
        }

        public static async Task<Embed> CreateLyricsEmbed(string title, string description, SocketGuildUser user, string thumburl)
        {
            Color color = SetColor();

            var embed = await Task.Run(() => new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(color)
                .WithThumbnailUrl(thumburl)
                .WithFooter(user.Username + "#" + user.Discriminator + " tarafından istek.", user.GetAvatarUrl()).Build());
            return embed;
        }

        public static async Task<Embed> CreateErrorEmbed(string source, string error)
        {
            var embed = await Task.Run(() => new EmbedBuilder()
                .WithTitle($"{source}")
                .WithDescription($"{error}")
                .WithColor(Color.Red).Build());
            return embed;
        }

        static readonly Random n = new Random();

        public static async Task<Embed> CreateUserEmbed(SocketGuildUser user)
        {
            var roles = new StringBuilder();

            foreach (var socketRole in user.Roles)
            {
                if (socketRole.Name != "@everyone")
                    roles.Append($"{socketRole.Mention}\n");
            }

            var role = roles.ToString();
            role = role == "" ? "`Rol Yok`" : role;

            string status = user.Status switch
            {
                UserStatus.Offline => "\\⚫️",
                UserStatus.Online => "\\🟢",
                UserStatus.Idle => "\\💤",
                UserStatus.AFK => "\\💤",
                UserStatus.DoNotDisturb => "\\⛔",
                UserStatus.Invisible => "\\⚫️",
                _ => "`Yok`",
            };

            Color color = SetColor();

            string activity;
            if (user.Activity is null)
                activity = "Yok";
            else
                activity = user.Activity.ToString();

            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>
            {
                new EmbedFieldBuilder
                {
                    Name = "Discorda Katılma Tarihi",
                    Value = $"`{user.CreatedAt.DateTime.ToString("dd MMM yyyy HH:mm")}`",
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Discorda Katılma Tarihi",
                    Value = $"`{user.JoinedAt.Value.DateTime.ToString("dd MMM yyyy HH:mm")}`",
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Aktivite",
                    Value = $"`{activity.Trim()}`",
                    IsInline = false
                },
                new EmbedFieldBuilder
                {
                    Name = "Roller",
                    Value = roles,
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Durum",
                    Value = status,
                    IsInline = true
                }
            };

            var embed = await Task.Run(() => new EmbedBuilder
            {
                Fields = fields,
                ThumbnailUrl = user.GetAvatarUrl(ImageFormat.Auto, 512),
                Footer = new EmbedFooterBuilder { Text = $"{user.Username}#{user.Discriminator}", IconUrl = user.GetAvatarUrl() },
                Color = color
            });

            return embed.Build();
        }

        public static Color SetColor()
        {
            int r = n.Next(0, 256);
            int g = n.Next(0, 256);
            int b = n.Next(0, 256);

            return new Color(r, g, b);

        }

    }
}
