using Discord;
using Discord.Commands;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Template.Utilities;

namespace Template.Modules
{
    public class Configuration : ModuleBase<SocketCommandContext>
    {
        private readonly Servers _servers;
        private readonly Ranks _ranks;
        private readonly AutoRoles _autoRoles;
        private readonly ServerHelper _serverHelper;

        public Configuration(Servers servers, Ranks ranks, AutoRoles autoRoles, ServerHelper serverHelper)
        {
            _servers = servers;
            _ranks = ranks;
            _autoRoles = autoRoles;
            _serverHelper = serverHelper;
        }

        [Command("prefix", RunMode = RunMode.Async)]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task Prefix(string prefix = null)
        {
            if (prefix == null)
            {
                var guildPrefix = await _servers.GetGuildPrefix(Context.Guild.Id) ?? ">";
                await ReplyAsync($"Şuanki Prefix : `{guildPrefix}`.");
                return;
            }

            if (prefix.Length > 8)
            {
                await ReplyAsync("Prefix'in maksimum uzunluğu 8 karakter olabilir.");
                return;
            }

            await _servers.ModifyGuildPrefix(Context.Guild.Id, prefix);
            await ReplyAsync($"Prefix'iniz `{prefix}` ile değiştirildi.");
            await _serverHelper.SendLogAsync(Context.Guild, "Prefix Değiştirildi", $"{Context.User.Mention} prefixi değiştirdi, yeni prefix : `{prefix}`.");
        }

        [Command("roller", RunMode = RunMode.Async)]
        public async Task Ranks()
        {
            var ranks = await _serverHelper.GetRanksAsync(Context.Guild);
            if(ranks.Count == 0)
            {
                await ReplyAsync("Sunucuda şuan rank bulunmamakta.");
                return;
            }

            await Context.Channel.TriggerTypingAsync();

            string description = "This message lists all available ranks.\nIn order to add a rank, you can use the name or ID of the rank.";
            foreach(var rank in ranks)
            {
                description += $"\n{rank.Mention} ({rank.Id})";
            }

            await ReplyAsync(description);
        }

        [Command("rolekle", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task AddRank([Remainder]string name)
        {
            await Context.Channel.TriggerTypingAsync();
            var ranks = await _serverHelper.GetRanksAsync(Context.Guild);

            var role = Context.Guild.Roles.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if(role == null)
            {
                await ReplyAsync("That role does not exist!");
                return;
            }

            if(role.Position > Context.Guild.CurrentUser.Hierarchy)
            {
                await ReplyAsync("That role has a higher position than the bot!");
                return;
            }

            if(ranks.Any(x => x.Id == role.Id))
            {
                await ReplyAsync("That role is already a rank!");
                return;
            }

            await _ranks.AddRankAsync(Context.Guild.Id, role.Id);
            await ReplyAsync($"The role {role.Mention} has been added to the ranks!");
        }

        [Command("rolsil", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task DelRank([Remainder]string name)
        {
            await Context.Channel.TriggerTypingAsync();
            var ranks = await _serverHelper.GetRanksAsync(Context.Guild);

            var role = Context.Guild.Roles.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (role == null)
            {
                await ReplyAsync("That role does not exist!");
                return;
            }

            if(ranks.Any(x => x.Id != role.Id))
            {
                await ReplyAsync("That role is not a rank yet!");
                return;
            }

            await _ranks.RemoveRankAsync(Context.Guild.Id, role.Id);
            await ReplyAsync($"The role {role.Mention} has been removed from the ranks!");
        }

        [Command("otoroller", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AutoRoles()
        {
            var autoRoles = await _serverHelper.GetAutoRolesAsync(Context.Guild);
            if (autoRoles.Count == 0)
            {
                await ReplyAsync("This server does not yet have any autoroles!");
                return;
            }

            await Context.Channel.TriggerTypingAsync();

            string description = "This message lists all autoroles.\nIn order to remove an autorole, use the name or ID.";
            foreach (var autoRole in autoRoles)
            {
                description += $"\n{autoRole.Mention} ({autoRole.Id})";
            }

            await ReplyAsync(description);
        }

        [Command("otorolekle", RunMode = RunMode.Async)]
        [Alias("otorol","oto-rol")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task AddAutoRole([Remainder] string name)
        {
            await Context.Channel.TriggerTypingAsync();
            var autoRoles = await _serverHelper.GetAutoRolesAsync(Context.Guild);

            var role = Context.Guild.Roles.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (role == null)
            {
                await ReplyAsync("That role does not exist!");
                return;
            }

            if (role.Position > Context.Guild.CurrentUser.Hierarchy)
            {
                await ReplyAsync("That role has a higher position than the bot!");
                return;
            }

            if (autoRoles.Any(x => x.Id == role.Id))
            {
                await ReplyAsync("That role is already an autorole!");
                return;
            }

            await _autoRoles.AddAutoRoleAsync(Context.Guild.Id, role.Id);
            await ReplyAsync($"Sunucuya girenler artık otomatik olarak {role.Mention} rolünü alacak!");
        }


        [Command("otorolsil", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task DelAutoRole([Remainder] string name)
        {
            await Context.Channel.TriggerTypingAsync();
            var autoRoles = await _serverHelper.GetAutoRolesAsync(Context.Guild);

            var role = Context.Guild.Roles.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (role == null)
            {
                await ReplyAsync("That role does not exist!");
                return;
            }

            if (autoRoles.Any(x => x.Id != role.Id))
            {
                await ReplyAsync("That role is not an autorole yet!");
                return;
            }

            await _autoRoles.RemoveAutoRoleAsync(Context.Guild.Id, role.Id);
            await ReplyAsync($"The role {role.Mention} has been removed from the autoroles!");
        }

        [Command("yenikullaniciduyuru")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Welcome(string option = null, string value = null)
        {
            if(option == null && value == null)
            {
                var fetchedChannelId = await _servers.GetWelcomeAsync(Context.Guild.Id);
                if(fetchedChannelId == 0)
                {
                    await ReplyAsync("There has not been set a welcome channel yet!");
                    return;
                }

                var fetchedChannel = Context.Guild.GetTextChannel(fetchedChannelId);
                if(fetchedChannel == null)
                {
                    await ReplyAsync("There has not been set a welcome channel yet!");
                    await _servers.ClearWelcomeAsync(Context.Guild.Id);
                    return;
                }

                var fetchedBackground = await _servers.GetBackgroundAsync(Context.Guild.Id);

                if (fetchedBackground != null)
                    await ReplyAsync($"The channel used for the welcome module is {fetchedChannel.Mention}.\nThe background is set to {fetchedBackground}.");
                else
                    await ReplyAsync($"The channel used for the welcome module is {fetchedChannel.Mention}.");

                return;
            }

            if(option == "kanal" && value != null)
            {
                if(!MentionUtils.TryParseChannel(value, out ulong parsedId))
                {
                    await ReplyAsync("Please pass in a valid channel!");
                    return;
                }

                var parsedChannel = Context.Guild.GetTextChannel(parsedId);
                if(parsedChannel == null)
                {
                    await ReplyAsync("Please pass in a valid channel!");
                    return;
                }

                await _servers.ModifyWelcomeAsync(Context.Guild.Id, parsedId);
                await ReplyAsync($"Yeni Kullanıcı selamlama kanalı artık {parsedChannel.Mention}.");
                return;
            }

            if (option == "background" && value != null)
            {
                if (value == "clear")
                {
                    await _servers.ClearBackgroundAsync(Context.Guild.Id);
                    await ReplyAsync("Successfully cleared the background for this server.");
                    return;
                }

                await _servers.ModifyBackgroundAsync(Context.Guild.Id, value);
                await ReplyAsync($"Successfully modified the background to {value}.");
                return;
            }

            if(option == "clear" && value == null)
            {
                await _servers.ClearWelcomeAsync(Context.Guild.Id);
                await ReplyAsync("Successfully cleared the welcome channel.");
                return;
            }

            await ReplyAsync("You did not use this command properly.");
        }

        [Command("logkanali")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Logs(string value = null)
        {
            if (value == null)
            {
                var fetchedChannelId = await _servers.GetLogsAsync(Context.Guild.Id);
                if (fetchedChannelId == 0)
                {
                    await ReplyAsync("There has not been set a logs channel yet!");
                    return;
                }

                var fetchedChannel = Context.Guild.GetTextChannel(fetchedChannelId);
                if (fetchedChannel == null)
                {
                    await ReplyAsync("There has not been set a logs channel yet!");
                    await _servers.ClearLogsAsync(Context.Guild.Id);
                    return;
                }

                await ReplyAsync($"The channel used for the logs is set to {fetchedChannel.Mention}.");

                return;
            }

            if (value != "clear")
            {
                if (!MentionUtils.TryParseChannel(value, out ulong parsedId))
                {
                    await ReplyAsync("Please pass in a valid channel!");
                    return;
                }

                var parsedChannel = Context.Guild.GetTextChannel(parsedId);
                if (parsedChannel == null)
                {
                    await ReplyAsync("Please pass in a valid channel!");
                    return;
                }

                await _servers.ModifyLogsAsync(Context.Guild.Id, parsedId);
                await ReplyAsync($"Successfully modified the logs channel to {parsedChannel.Mention}.");
                return;
            }

            if (value == "clear")
            {
                await _servers.ClearLogsAsync(Context.Guild.Id);
                await ReplyAsync("Successfully cleared the logs channel.");
                return;
            }

            await ReplyAsync("You did not use this command properly.");
        }

        [Command("duyurularkanali")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Announcements(string value = null)
        {
            if (value == null)
            {
                var fetchedChannelId = await _servers.GetLogsAsync(Context.Guild.Id);
                if (fetchedChannelId == 0)
                {
                    await ReplyAsync("There has not been set a logs channel yet!");
                    return;
                }

                var fetchedChannel = Context.Guild.GetTextChannel(fetchedChannelId);
                if (fetchedChannel == null)
                {
                    await ReplyAsync("There has not been set a logs channel yet!");
                    await _servers.ClearLogsAsync(Context.Guild.Id);
                    return;
                }

                await ReplyAsync($"The channel used for the logs is set to {fetchedChannel.Mention}.");

                return;
            }

            if (value != "kaldir")
            {
                if (!MentionUtils.TryParseChannel(value, out ulong parsedId))
                {
                    await ReplyAsync("Please pass in a valid channel!");
                    return;
                }

                var parsedChannel = Context.Guild.GetTextChannel(parsedId);
                if (parsedChannel == null)
                {
                    await ReplyAsync("Please pass in a valid channel!");
                    return;
                }

                await _servers.GetAnnouncementsAsync(Context.Guild.Id, parsedId);
                await ReplyAsync($"{parsedChannel.Mention} adlı kanal duyurular kanalı olarak ayarlandı.");
                return;
            }

            if (value == "temizle")
            {
                await _servers.ClearLogsAsync(Context.Guild.Id);
                await ReplyAsync("Successfully cleared the logs channel.");
                return;
            }

            await ReplyAsync("You did not use this command properly.");
        }
    }
}
