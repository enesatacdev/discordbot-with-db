using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Template.Modules.Anime.MathSolverHelper;

namespace Template.Modules.Anime
{
    public class MathSolver : ModuleBase
    {
        [Command("anime-komutlari")]
        public async Task AnimeCommandsAsync()
        {
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithTitle($" Anime Resim/Gif'leri için yardım bilgileri.");
            eb.WithFooter(Context.Client.CurrentUser.Username.ToString());
            eb.WithDescription($"**Aşağıdaki komutları kullanarak resim/gif kategorilerini belirleyebilirsiniz.**");
            eb.WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl());
            eb.AddField("-18 (SFW) Komutlar","Aşağıda Bulunan Komutlar Cinsellik içermeyen görselleri getirir.", false);
            eb.AddField("```waifu , kedikiz , zorba , sarilmak , aglamak , opucuk , katil , mutlu```", "-18", false);
            eb.AddField("+18 (NSFW) Komutlar", "Aşağıda Bulunan Komutlar Cinsellik içeren görselleri getirir.", false);
            eb.AddField("+18", "```waifu , kedikiz , gay , sakso```", false);

            await ReplyAsync("", false, eb.Build());
        }


        [Command("anime", RunMode = RunMode.Async)]
        [Summary("Anime kızı resimleri :)")]
        public async Task AnimeGirlAsync(string category)
        {
            var client = new HttpClient();
            switch (category)
            {
                case "waifu":
                    category = "waifu";
                break;

                case "kedikiz":
                    category = "neko";
                    break;

                case "zorba":
                    category = "bully";
                    break;

                case "sarilmak":
                    category = "cuddle";
                    break;

                case "aglamak":
                    category = "cry";
                    break;

                case "opucuk":
                    category = "kiss";
                    break;

                case "katil":
                    category = "kill";
                    break;

                case "mutlu":
                    category = "happy";
                    break;

                default:
                    category = "waifu";
                    break;
            }

            var json = await client.GetStringAsync($"https://api.waifu.pics/sfw/{category}");
            var myPages = JsonConvert.DeserializeObject<Root>(json);
            var url = myPages.Url;

            var builder = new EmbedBuilder()
               .WithColor(new Color(33, 176, 255))
               .WithImageUrl(url)
               .WithCurrentTimestamp()
               .Build();
            await ReplyAsync(embed: builder);
        }

        [Command("anime18", RunMode = RunMode.Async)]
        [Summary("+18 Anime kızı resimleri :)")]
        public async Task AnimeGirl18Async(string category)
        {
            var client = new HttpClient();

            switch (category)
            {
                case "waifu":
                    category = "waifu";
                    break;

                case "kedikiz":
                    category = "neko";
                    break;

                case "trap":
                    category = "gay";
                    break;

                case "sakso":
                    category = "blowjob";
                    break;

                default:
                    category = "waifu";
                    break;
            }

            var json = await client.GetStringAsync($"https://api.waifu.pics/nsfw/{category}");
            var myPages = JsonConvert.DeserializeObject<Root>(json);
            var url = myPages.Url;

            var builder = new EmbedBuilder()
               .WithColor(new Color(33, 176, 255))
               .WithTitle("+18")
               .WithImageUrl(url)
               .WithCurrentTimestamp()
               .Build();
            await ReplyAsync(embed: builder);
        }
    }
}
