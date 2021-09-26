using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Template.Common;

namespace Template.Modules.Utility
{
    public class Utility : ModuleBase
    {

        [Command("sunucuyasi"), Summary("Get the servers birth date."), RequireContext(ContextType.Guild)]
        public async Task Age()
        {
            await ReplyAsync(Context.Guild.CreatedAt.Date.ToLongDateString());
        }

        [Command("katilmatarihim"), Summary("Get the date the user joined.")]
        public async Task Joined()
        {
            if (Context.User is IGuildUser user && user.JoinedAt.HasValue)
            {
                await ReplyAsync(user.JoinedAt.Value.Date.ToLongDateString());
            }
        }

        [Command("katilmatarihi"), Summary("Get the date the user joined.")]
        public async Task Joined(IGuildUser user)
        {
            if (user.JoinedAt.HasValue)
            {
                await ReplyAsync(user.JoinedAt.Value.Date.ToLongDateString());
            }
        }
        [Command("ceviri"), Summary("Google Translate in discord.")]
        public async Task Translate(string from, string to, [Remainder] string text)
        {
            string textOrg = text;
            from = Uri.EscapeDataString(from);
            to = Uri.EscapeDataString(to);
            text = Uri.EscapeDataString(text);
            var http = new HttpClient();
            JArray json = JArray.Parse(await http.GetStringAsync($"https://translate.google.com/translate_a/single?client=gtx&sl={from}&tl={to}&dt=t&q={text}"));

            if (json.HasValues && json[0].HasValues && json[0][0].HasValues)
            {
                await ReplyAsync(embed: await EmbedHandler.TranslateEmbed(from, to, textOrg, json[0][0][0].Value<string>()));
            }
        }

        [Command("ingilizce-ceviri"), Summary("Google Translate in discord.")]
        public async Task Translate([Remainder] string text)
        {
            string textOrg = text;
            string from = Uri.EscapeDataString("tr");
            string to = Uri.EscapeDataString("en");
            text = Uri.EscapeDataString(text);
            var http = new HttpClient();
            JArray json = JArray.Parse(await http.GetStringAsync($"https://translate.google.com/translate_a/single?client=gtx&sl={from}&tl={to}&dt=t&q={text}"));

            if (json.HasValues && json[0].HasValues && json[0][0].HasValues)
            {
                await ReplyAsync(embed: await EmbedHandler.TranslateEmbed(from, to, textOrg, json[0][0][0].Value<string>()));
            }
        }
        [Command("ss-al"), Summary("Screenshot a website.")]
        public async Task Screenshot(string url)
        {
            if (!url.StartsWith("http"))
            {
                await Context.Channel.SendMessageAsync("Girdiğiniz link yanlış");
                return;
            }

            await this.ReplyAsync("", false, new EmbedBuilder().WithImageUrl($"https://api.microlink.io/?url={Uri.EscapeDataString(url)}&screenshot=true&meta=false&embed=screenshot.url").Build());
        }

        [Command("status"), Summary("Check discord server status.")]
        public async Task Status()
        {

            var client = new HttpClient();
            var getString = await client.GetStringAsync($"https://srhpyqt94yxb.statuspage.io/api/v2/summary.json");
            var json = JsonConvert.DeserializeObject<StatusPageJson>(getString);

            EmbedBuilder builder = new EmbedBuilder()
            {
                Title = json.Status.Description,
                Color = new Discord.Color(0x738BD7),

                Fields = json.Components.Select(x => new EmbedFieldBuilder().WithIsInline(true).WithName(x.Name).WithValue(x.Status)).ToList()
            };

            await ReplyAsync("", false, builder.Build());
        }
    }
}
