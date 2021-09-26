using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Template.Services;
using Template.Extensions;
using Newtonsoft.Json;
using System.Net.Http;

namespace Template.Modules.Lewd
{
    [RequireNsfw]
    public class Lewd : ModuleBase
    {
        [Command("lewd"), Summary("Full of Haricy.")]
        public async Task Gelbooru(params string[] tags)
        {
            string safe = string.Join("+", tags);
            var client = new HttpClient();
            var json = await client.GetStringAsync($"https://gelbooru.com/index.php?page=dapi&s=post&q=index&limit=60&json=1&tags=sort:random+-loli+score:>=3+{safe}");
            var Posts = JsonConvert.DeserializeObject<List<GelbooruJson>>(json);


            if (Posts != null && Posts.Count > 0)
            {
                foreach (GelbooruJson post in Posts)
                {
                    if (post.Tags.Contains("loli"))
                    {
                        continue;
                    }

                    if (post.FileUrl.EndsWith(".webm"))
                    {
                        await ReplyAsync(post.FileUrl);
                    }
                    else
                    {
                        await ReplyAsync("", false, new EmbedBuilder()
                        {
                            Color = Color.Blue,
                            ImageUrl = post.FileUrl
                        }.Build());
                    }

                    break;
                }
            }
            else
            {
                await ReplyAsync("I could not find any images with provided tags.");
            }
        }

        [Command("rule34"), Summary("No checks are done to ensure quality.")]
        public async Task Rule34()
        {
            var client = new HttpClient();
            var url = await client.GetStringAsync($"https://rule34.xxx/index.php?page=post&s=random");

            if (url.EndsWith(".webm"))
            {
                await ReplyAsync(url);
            }
            else
            {
                await ReplyAsync("", false, new EmbedBuilder()
                {
                    Color = Color.Blue,
                    ImageUrl = url
                }.Build());
            }
            
        }

    }

}
