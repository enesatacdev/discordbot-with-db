using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Template.Modules.Fun
{
    public class Dice : ModuleBase
    {
        private static readonly string[] DiceIds = new string[6] { "6Zzf1KL", "RKCQ1w5", "P2F567A", "cx94cKE", "zyGoZXz", "a9PEYtY" };

        [Command("zar"), Summary("Roll a Six-sided die")]
        public async Task DiceAsync()
        {
            Random rastgele = new Random();
            int sayi = rastgele.Next(1,7);
            int Roll = sayi;
            string url = $"https://i.imgur.com/{DiceIds[Roll - 1]}.png";

            await Context.Channel.SendMessageAsync("", false, new EmbedBuilder()
            {
                Title = "Zar",
                Description = $"Attığınız Zar : {Roll}",
                Color = new Color(0x527BBB),
                ThumbnailUrl = url
            }.Build());
        }
    }
}
