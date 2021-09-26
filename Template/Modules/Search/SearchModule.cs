using Discord;
using Discord.Commands;
using Interactivity;
using Interactivity.Pagination;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Template.Services;

namespace Template.Modules.Search
{
    public class SearchModule : ModuleBase
    {
        public InteractivityService Interactivity { get; set; }

        [Command("mal",RunMode=RunMode.Async), Alias("anime"), Summary("Search my anime list.")]
        public async Task MyAnimeList([Remainder] string query)
        {
            var client = new HttpClient();
            var getString = await client.GetStringAsync($"https://api.jikan.moe/v3/search/anime?q={Uri.EscapeDataString(query)}");
            MALJson data = JsonConvert.DeserializeObject<MALJson>(getString);

            if (data != null)
            {
                IEnumerable<EmbedBuilder> builders = data.Results.Select(x => new EmbedBuilder().WithTitle(x.Title).WithDescription(x.Synopsis).WithThumbnailUrl(x.ImageUrl).WithUrl(x.Url));
                List<string> titles = data.Results.Select(x => x.Title).ToList();
                List<string> description = data.Results.Select(x => x.Synopsis).ToList();
                List<string> thumbnailUrl = data.Results.Select(x => x.ImageUrl).ToList();
                List<string> url = data.Results.Select(x => x.Url).ToList();
                List<float> score = data.Results.Select(x => x.Score).ToList();
                var pages = new PageBuilder[builders.Count()];
                if (builders.Count() > 10)
                {
                    pages = new PageBuilder[10];
                }

                for (int i = 0; i < pages.Length; i++)
                {
                    pages[i] = new PageBuilder()
                        .WithTitle(titles[i])
                        .WithThumbnailUrl(thumbnailUrl[i])
                        .WithDescription(description[i])
                        .AddField("Skor",score[i],false)
                        .AddField("Url",url[i],false)
                        .WithFooter($" Sayfa {i + 1}/{pages.Length}",thumbnailUrl[i]);
                }

                var paginator = new StaticPaginatorBuilder()
                    .WithPages(pages)
                    .WithFooter(PaginatorFooter.None)
                    .Build();

                await Interactivity.SendPaginatorAsync(paginator, Context.Channel, TimeSpan.FromMinutes(2));

               
            }
        }

        //[Command("trace"), Summary("Trace.moe in discord.")]
        //public async Task TraceMoe(string url)
        //{
        //    var client = new HttpClient();
        //    var getString = await client.GetStringAsync($"https://trace.moe/api/search?url=" + Uri.EscapeDataString(url));
        //    TraceMoeJson json = JsonConvert.DeserializeObject<TraceMoeJson>(getString);

        //    if (json.Docs != null && json.Docs.Any())
        //    {
        //        IEnumerable<EmbedBuilder> builders = json.Docs.Select(x => new EmbedBuilder().WithTitle(x.Title).WithDescription(x.TitleEnglish).WithThumbnailUrl(x.ImageThumbnail));

        //        await pagination.SendMessageAsync(Context.Channel, new PaginatedMessage(builders, null, Color.Green, Context.User, new AppearanceOptions()
        //        {
        //            Timeout = TimeSpan.FromMinutes(3),
        //            Style = DisplayStyle.Selector
        //        }));
        //    }
        //    else
        //    {
        //        await Context.Channel.SendMessageAsync("Sonuç bulunamadı");
        //    }
        //}

       

        [Command("mc"), Summary("Get minecraft profile info.")]
        public async Task MCName(string username)
        {
            var client = new HttpClient();
            var getString = await client.GetStringAsync($"https://api.mojang.com/users/profiles/minecraft/{Uri.EscapeDataString(username)}");
            var profile = JsonConvert.DeserializeObject<MinecraftProfileJson>(getString);
            if (profile != null)
            {
                var profileGetString = await client.GetStringAsync($"https://api.mojang.com/user/profiles/{profile.ID}/names");
                List<MinecraftNameHistoryJson> names = JsonConvert.DeserializeObject<List<MinecraftNameHistoryJson>>(profileGetString);
                EmbedBuilder builder = new EmbedBuilder()
                {
                    Title = profile.Name,
                    Description = profile.ID,
                    Color = Color.Green,
                    ThumbnailUrl = $"https://crafatar.com/avatars/{profile.ID}.png?overlay=true"
                };

                if (names != null && names.Count > 0)
                {
                    foreach (MinecraftNameHistoryJson name in names.Reverse<MinecraftNameHistoryJson>())
                    {
                        string date = name.ChangedToAt.HasValue ? new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(name.ChangedToAt.Value).ToString("MM/dd/yyyy") : "Tarih Bulunamadı.";
                        builder.AddField(name.Name, date, true);
                    }
                }

                await ReplyAsync("", false, builder.Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync("Girmiş olduğunuz kullanıcı ismine göre herhangi bir oyuncu bulunamadı.");
            }
        }

    }
}
