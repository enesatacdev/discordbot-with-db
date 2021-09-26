using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Template.Common;
using Template.Utilities;
using static Template.Modules.Anime.MathSolverHelper;

namespace Template.Modules.Nath
{
    public class MathSolver : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<ExampleModule> _logger;
        private readonly ServerHelper _serverHelper;

        public MathSolver(ILogger<ExampleModule> logger, ServerHelper serverHelper)
        {
            _logger = logger;
            _serverHelper = serverHelper;
        }
        [Command("matematik", RunMode = RunMode.Async)]
        [Summary("Matematik işlemleri çözer.")]
        public async Task MathSolverAsync([Remainder] string math)
        {
            var client = new HttpClient();
            var sonuc = await client.GetStringAsync($"https://api.mathjs.org/v4/?expr={math}");

            var message = await Context.Channel.SendSuccessAsync("Çözüldü", $"Matematik işlemi ve sonucu ```{math} = {sonuc}```");
            await _serverHelper.SendLogAsync(Context.Guild, "Matematik İşlemi Çözüldü", $"{Context.User.Mention} matematiği beceremediği için onun yerine biz çözdük. \n ```{math} = {sonuc}```");

        }


    }
}
