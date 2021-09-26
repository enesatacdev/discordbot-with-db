using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Interactivity;
using Interactivity.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Template.Common;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;
using Victoria.Responses.Rest;

namespace Template.Modules.Music
{

    [Name("Müzik Komutları")]
    [Summary("Müzik Komutları")]
    public class Music : ModuleBase<SocketCommandContext>
    {
        private readonly LavaNode _lavaNode;
        public InteractivityService Interactivity { get; set; }
        public Music(LavaNode lavaNode)
        {
            _lavaNode = lavaNode;
        }

        static string lastQueued = "";

        public static bool CheckURLValid(string source) => Uri.TryCreate(source, UriKind.Absolute, out Uri uriResult) && uriResult.Scheme == Uri.UriSchemeHttps;

        public static List<LavaTrack> queue = new List<LavaTrack>();

        [Command("baglan", RunMode = RunMode.Async)]
        [Alias("katıl", "katil")]
        [Summary("Bot'un ses kanalına katılmasını sağlar.")]
        public async Task JoinAsync()
        {
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("Şuanda başka bir kanala bağlıyım!");
                return;
            }

            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("Bir ses kanalına bağlı olman gerekiyor!!");
                return;
            }

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [Command("oynat", RunMode = RunMode.Async)]
        [Alias("çal", "muzik", "müzik", "baslat", "başlat", "o")]
        [Summary("Bot'un Müzik Çalmasını Sağlar")]
        public async Task PlayAsync([Remainder] string query)
        {

            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("Bir Ses Kanalına Bağlı Olmak Zorundasınız!");
                return;
            }
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("Benimle Aynı Ses Kanalında Bulunman Gerekiyor!");
                return;
            }

            if (string.IsNullOrWhiteSpace(query))
            {
                await ReplyAsync("Oynatmak istediğiniz parçayı tanımlayınız!");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"{voiceState.VoiceChannel.Name} kanalına katıldım!");
            }

            var searchResponse = await _lavaNode.SearchYouTubeAsync(query);
            if (CheckURLValid(query) == true)
            {
                searchResponse = await _lavaNode.SearchAsync(query);
            }


            if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                searchResponse.LoadStatus == LoadStatus.NoMatches)
            {
                await ReplyAsync($"Aradığınız `{query}` parçayı bulamadım! .");
                return;
            }


            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {

                var track = searchResponse.Tracks[0];
                player.Queue.Enqueue(track);
                await ReplyAsync(embed: await EmbedHandler.NextSong(track.Title, track.Duration.ToString(), track.FetchArtworkAsync().ToString()));
                lastQueued = query;

            }
            else
            {
                var track = searchResponse.Tracks[0];
                await player.PlayAsync(track);

                var builder = new EmbedBuilder()
                    .WithTitle($"Şuanda Çalınan Parça")
                    .WithThumbnailUrl(track.FetchArtworkAsync().ToString())
                    .AddField("Parça Adı", track.Title, false)
                    .AddField("Parça Uzunluğu", track.Duration.ToString(), false)
                    .WithColor(new Color(33, 176, 252));
                var embed = builder.Build();
                var message = await Context.Channel.SendMessageAsync(null, false, embed);
                lastQueued = query;
            }

        }

        [Command("karistir", RunMode = RunMode.Async)]
        [Alias("shuffle", "listeyi-karistir", "listeyi karıştır", "listeyi-karıştır", "k")]
        [Summary("Listede'ki parçaları karıştırır")]
        public async Task ShuffleTrackAsync()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("Bir Ses Kanalına Bağlı Olmak Zorundasınız!");
                return;
            }
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("Bir Ses Kanalına Bağlı Değilim!");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("Benimle Aynı Ses Kanalında Bulunman Gerekiyor!");
                return;
            }
            var queue = player.Queue.ToList();
            var queueCount = player.Queue.Count;

            if (queueCount == 0)
            {
                await Context.Channel.SendMessageAsync("Kuyrukta herhangi bir parça yok!");
                return;
            }
            else if (queueCount == 1)
            {
                await Context.Channel.SendMessageAsync("Kuyrukta bir parça olduğundan karıştıramıyorum!");
                return;
            }
            else
            {
                await Context.Channel.SendMessageAsync("Kuyruktaki parçalar karıştırıldı!");
                player.Queue.Shuffle();
                return;
            }
        }

        [Command("duraklat", RunMode = RunMode.Async)]
        [Alias("d")]
        [Summary("Parçayı duraklatır.")]
        public async Task PauseMusicAsync()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("Bir Ses Kanalına Bağlı Olmak Zorundasınız!");
                return;
            }
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("Bir Ses Kanalına Bağlı Değilim!");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("Benimle Aynı Ses Kanalında Bulunman Gerekiyor!");
                return;
            }
            if (player.PlayerState == PlayerState.Paused)
            {
                await ReplyAsync("Parça Zaten Durdurulmuş Durumda!");
                return;
            }

            await player.PauseAsync();
            await ReplyAsync("Parça Durduruldu");
        }



        [Command("kuyruk", RunMode = RunMode.Async)]
        [Alias("parcalar", "liste", "listedekiler", "queue", "q")]
        [Summary("Listede'ki parçaları listeler.")]
        public async Task QueueList()
        {
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed("Bağlantı Hatası", "Bir ses kanalına bağlı değilim."));
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            int user = player.VoiceChannel.GetUsersAsync().FlattenAsync().Result.Count(x => !x.IsBot);
            List<string> tracks = new List<string>();
            StringBuilder builder = new StringBuilder();

            if (player.PlayerState is PlayerState.Playing)
            {
                if (player.Track.Duration.Hours == 0)
                {
                    builder.Append($"Oynatılıyor 🎶 [{player.Track.Title}]({player.Track.Url}) | `{player.Track.Position:mm\\:ss}/{player.Track.Duration:mm\\:ss}`\n\n");
                }
                else
                {
                    builder.Append($"Oynatılıyor 🎶 [{player.Track.Title}]({player.Track.Url}) | `{player.Track.Position:hh\\:mm\\:ss}/{player.Track.Duration:hh\\:mm\\:ss}`\n\n");
                }
            }
            else if (player.PlayerState is PlayerState.Paused)
            {
                if (player.Track.Duration.Hours == 0)
                {
                    builder.Append($"Durduruldu ⏸️ [{player.Track.Title}]({player.Track.Url}) | `{player.Track.Position:mm\\:ss}/{player.Track.Duration:mm\\:ss}`\n\n");
                }
                else
                {
                    builder.Append($"Durduruldu ⏸️ [{player.Track.Title}]({player.Track.Url}) | `{player.Track.Position:hh\\:mm\\:ss}/{player.Track.Duration:hh\\:mm\\:ss}`\n\n");
                }
            }


            try
            {

                int trackNum = 1;
                foreach (var track in player.Queue)
                {
                    if (track.Duration.Hours == 0)
                    {
                        builder.Append($"`{trackNum}.` [{track.Title}]({track.Url}) | `{track.Duration:mm\\:ss}`\n\n");
                    }
                    else
                    {
                        builder.Append($"`{trackNum}.` [{track.Title}]({track.Url}) | `{track.Duration:hh\\:mm\\:ss}`\n\n");
                    }

                    if (trackNum % 10 == 0)
                    {
                        tracks.Add(builder.ToString());
                        builder.Clear();
                    }

                    trackNum++;
                }

                if (builder.Length != 0)
                    tracks.Add(builder.ToString());

                if (tracks.Count == 1)
                {
                    Interactivity.DelayedDeleteMessageAsync(await Context.Channel.SendMessageAsync(embed: new EmbedBuilder().WithAuthor($"Kuyruktaki parça sayısı {player.Queue.Count}")
                        .WithTitle("Oynatma Listesi")
                        .WithDescription(tracks[0])
                        .WithFooter($" Sayfa 1/1", Context.User.GetAvatarUrl()).Build()), TimeSpan.FromMinutes(2));

                    return;
                }

                var pages = new PageBuilder[tracks.Count];

                for (int i = 0; i < pages.Length; i++)
                {
                    pages[i] = new PageBuilder().WithAuthor($"{player.Queue.Count} Parça").WithTitle("Oynatma Listesi")
                            .WithDescription(tracks[i])
                            .WithFooter($" Sayfa {i + 1}/{pages.Length}", Context.User.GetAvatarUrl());
                }

                var paginator = new StaticPaginatorBuilder()
                    .WithUsers(Context.User)
                    .WithPages(pages)
                    .WithFooter(PaginatorFooter.None)
                    .Build();

                await Interactivity.SendPaginatorAsync(paginator, Context.Channel, TimeSpan.FromMinutes(2));
            }
            catch (Exception ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed("Oynatma Listesi", "Liste Boş"));
                return;
            }
        }

        [Command("atla", RunMode = RunMode.Async)]
        [Alias("skip", "s", "gec", "a")]
        [Summary("Listede bulunan sıradaki parçaya atlar.")]
        public async Task SkipMusicAsync(int amount = 1)
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("Bir Ses Kanalına Bağlı Olmak Zorundasınız!");
                return;
            }
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("Bir Ses Kanalına Bağlı Değilim!");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("Benimle Aynı Ses Kanalında Bulunman Gerekiyor!");
                return;
            }

            for (int i = 0; i < amount; i++)
            {
                await player.SkipAsync();
            }

            await ReplyAsync("Parça Atlandı!");
        }

        [Command("devam-et", RunMode = RunMode.Async)]
        [Alias("continue", "d-e", "devamet", "devam-ettir", "devam ettir", "devam-ettir")]
        [Summary("Durdurulmuş parçayı devam ettirir.")]
        public async Task ResumeMusicAsync()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("Bir Ses Kanalına Bağlı Olmak Zorundasınız!");
                return;
            }
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("Bir Ses Kanalına Bağlı Değilim!");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("Benimle Aynı Ses Kanalında Bulunman Gerekiyor!");
                return;
            }
            if (player.PlayerState != PlayerState.Paused)
            {
                await ReplyAsync("Parça Şuanda Devam Etmekte!");
                return;
            }
            else
            {
                await player.ResumeAsync();
                await ReplyAsync("Parça Devam Ettiriliyor");
            }


        }

        [Command("siktir-git", RunMode = RunMode.Async)]
        public async Task SiktirGitAsync(IVoiceChannel channel = null)
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("Beni kovabilmek için benimle aynı ses kanalında olman gerekiyor!");
                return;
            }
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("Beni kovabiliceğin bir ses kanalına bağlı değilim!");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("Beni kovabilmek için benimle aynı ses kanalında olman gerekiyor!");
                return;
            }

            await _lavaNode.LeaveAsync(voiceState.VoiceChannel);
            await ReplyAsync($"{voiceState.VoiceChannel.Name} kanalından Kovuldum!");

        }

        [Command("durdur", RunMode = RunMode.Async)]
        [Alias("stop", "kapat", "muzigi-kapat")]
        [Summary("Parçayı durdurur.")]
        public async Task StopTrackAsync()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("Bir Ses Kanalına Bağlı Olmak Zorundasınız!");
                return;
            }
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("Bir Ses Kanalına Bağlı Değilim!");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("Benimle Aynı Ses Kanalında Bulunman Gerekiyor!");
                return;
            }
            if (player.PlayerState == PlayerState.Stopped)
            {
                await ReplyAsync("Parça Zaten Durdurulmuş Durumda!");
                return;
            }

            await player.PauseAsync();
            await ReplyAsync("Parça Kapatıldı!");
        }

        [Command("yer-degistir", RunMode = RunMode.Async)]
        [Summary("Listede'ki parçaların yerini değiştirir.")]
        [Alias("degistir", "kuyruk-degistir", "kuyruktakileri-degistir")]
        public async Task TrackMoveAsync(int toBeChanged, int modified)
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("Bir Ses Kanalına Bağlı Olmak Zorundasınız!");
                return;
            }
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("Bir Ses Kanalına Bağlı Değilim!");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("Benimle Aynı Ses Kanalında Bulunman Gerekiyor!");
                return;
            }

            List<LavaTrack> queue = player.Queue.ToList();
            var queueCount = player.Queue.Count;
            if (queueCount < 2)
            {
                await Context.Channel.SendMessageAsync("Yerlerini değiştirebiliceğim parçalar bulunmamakta!");
                return;
            }
            var oldQueue = queue[toBeChanged - 1];
            queue[toBeChanged - 1] = queue[modified - 1];
            queue[modified - 1] = oldQueue;

            player.Queue.RemoveRange(0, queueCount);


            foreach (var track in queue)
            {
                player.Queue.Enqueue(track);
            }

            await Context.Channel.SendMessageAsync($"{oldQueue.Title} {modified}. Sıraya Taşındı!");
        }

        [Command("parca-sil", RunMode = RunMode.Async)]
        [Summary("Listede'ki parçayı listeden kaldırır.")]
        [Alias("sil", "kuyruktan-sil", "cikar", "kuyruktan-cikar", "listeden-cikar", "çıkar", "kuyruktan-çıkar", "kaldır", "kaldir")]
        public async Task DeleteTrackFromQueueAynsc(int toBeDeleted)
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("Bir Ses Kanalına Bağlı Olmak Zorundasınız!");
                return;
            }
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("Bir Ses Kanalına Bağlı Değilim!");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("Benimle Aynı Ses Kanalında Bulunman Gerekiyor!");
                return;
            }

            List<LavaTrack> queue = player.Queue.ToList();
            var queueCount = player.Queue.Count;
            if (queueCount < 1)
            {
                await Context.Channel.SendMessageAsync("Kuyrukta Silebiliceğim bir parça yok!");
                return;
            }
            var tobedeletedtrack = queue[toBeDeleted - 1];
            await Context.Channel.SendMessageAsync($"{tobedeletedtrack.Title} Kuyruktan Silindi!");
            player.Queue.Remove(tobedeletedtrack);

        }

        private static readonly IEnumerable<int> Range = Enumerable.Range(1900, 2000);

        [Command("lyrics", RunMode = RunMode.Async)]
        [Summary("Çalmakta olan şarkının/parça'nın sözlerini gösterir.")]
        [Alias("sozler", "şarkı-sözleri", "şarkı sözleri", "sarki-sozleri")]
        public async Task ShowGeniusLyrics()
        {
            LavaPlayer player = null;

            if (_lavaNode.HasPlayer(Context.Guild))
            {
                player = _lavaNode.GetPlayer(Context.Guild);
            }

            
            try
            {
                await Context.Channel.SendMessageAsync($"**Lyrics Aranıyor** 🔎 `{player.Track.Title}`");
                string lyrics = await LyricsService.GetLyricsFromGenius(player.Track.Title);

                if (lyrics.Length > 2000)
                {
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateLyricsEmbed($"{LyricsService.Title} Lyrics", lyrics.Substring(0, 1900) + $"...\n\nFor the full lyrics, [click here]({LyricsService.TrackURL})", Context.User as SocketGuildUser, LyricsService.TrackImage));
                    return;
                }

                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateLyricsEmbed($"{LyricsService.Title} Lyrics", lyrics, Context.User as SocketGuildUser, LyricsService.TrackImage));
            }
            catch (Exception ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(null, "Birşeyler Ters Gitti"));
            }

        }

        [Command("Ara", RunMode = RunMode.Async)]
        [Alias("youtube", "parca-ara", "parça ara", "arama", "bul")]
        [Summary("Youtube'dan içerik aramanızı sağlar")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SearchAsync([Remainder] string query = null)
            => await SearchAsyncMethod(Context.Channel as ITextChannel, Context.User as SocketGuildUser, query, Context.Guild);

        private async Task<SearchResponse> SearchQueryFromYoutube(string query)
            => Uri.IsWellFormedUriString(query, UriKind.Absolute) ? await _lavaNode.SearchAsync(query) : await _lavaNode.SearchYouTubeAsync(query);

        readonly List<SocketUser> names = new List<SocketUser>();
        readonly List<LavaTrack[]> querys = new List<LavaTrack[]>();
        readonly List<IUserMessage> messages = new List<IUserMessage>();
        public async Task SearchAsyncMethod(ITextChannel textChannel, SocketGuildUser user, string query, IGuild guild)
        {


            if (user.VoiceChannel is null)
            {
                await textChannel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed("Arama", "Bir ses kanalında olmak zorundasın."));
                return;
            }

            if (query == null)
            {
                await textChannel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed("Yanlış Kullanım :x:",
                    "Lütfen aramak istediğiniz şarkı/parça'nın adını giriniz. \n\n.Ara [query]"));
                return;
            }

            await _lavaNode.JoinAsync(user.VoiceChannel, textChannel);
            var player = _lavaNode.GetPlayer(guild);
            int users = player.VoiceChannel.GetUsersAsync().FlattenAsync().Result.Count(x => !x.IsBot);

            try
            {
                if (user.VoiceChannel == player.VoiceChannel || ((user.VoiceChannel != player.VoiceChannel) && (users == 0)) || player.Track is null)
                {
                    if (names.Contains(user))
                    {
                        await textChannel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed("İptal",
                                "\"iptal\" yazarak aramayı iptal edebilirsiniz."));
                        return;
                    }

                    StringBuilder TrackList = new StringBuilder();

                    var search = await SearchQueryFromYoutube(query);

                    if (search.LoadStatus == LoadStatus.NoMatches)
                    {
                        await textChannel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed("Arama",
                            $"\"{query}\" Hakkında herhangi birşey bulamadım"));
                        return;
                    }
                    LavaTrack[] tracks = new LavaTrack[10];

                    if (search.Tracks.Count < 10)
                    {
                        int a = 0;
                        tracks = new LavaTrack[search.Tracks.Count];
                        foreach (var item in search.Tracks)
                        {
                            tracks[a] = item;
                            a++;
                        }
                    }

                    if (tracks.Length == 10)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            tracks[i] = search.Tracks[i];
                        }
                    }
                    for (int i = 0; i < tracks.Length; i++)
                    {
                        if (tracks[i].Duration.Hours == 0)
                            TrackList.Append($"`{i + 1}.` [{tracks[i].Title}]({tracks[i].Url}) [`{tracks[i].Duration:mm\\:ss}`]\n\n");
                        else
                            TrackList.Append($"`{i + 1}.` [{tracks[i].Title}]({tracks[i].Url}) [`{tracks[i].Duration:hh\\:mm\\:ss}`]\n\n");
                    }

                    var msg = await textChannel.SendMessageAsync("Lütfen Bir Parça Seçiniz.\n",
                        embed: await EmbedHandler.CreateBasicEmbed("Ara", $"{TrackList}", user));

                    messages.Add(msg);
                    names.Add(user);
                    querys.Add(tracks);



                    var response = await Interactivity.NextMessageAsync(x => x.Author == Context.User);
                    if (response == null)
                    {
                        await ReplyAsync("Zamanında parça seçmediniz.");
                        return;
                    }

                    try
                    {
                        int selectedMusic = Convert.ToInt32(response.Value.Content);
                        if (response != null && response.Value.Content != "iptal")
                        {
                            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
                            {
                                var searchResponse = await _lavaNode.SearchAsync(tracks[selectedMusic - 1].Url);
                                var track = searchResponse.Tracks[0];
                                player.Queue.Enqueue(track);
                                await ReplyAsync(embed: await EmbedHandler.NextSong(track.Title, track.Duration.ToString(), track.FetchArtworkAsync().ToString()));

                            }
                            else
                            {
                                await PlayAsync(tracks[selectedMusic - 1].Title);
                            }
                        }
                        else if (response != null && response.Value.Content.ToLower() == "iptal")
                        {
                            return;
                        }
                    }
                    catch
                    {
                        await ReplyAsync("Arama sonuçlarında çıkan parçalardan birinin numarasını girmelisin.");
                        return;
                    }




                }
                if (user.VoiceChannel != player.VoiceChannel)
                {
                    await textChannel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed("Hata!", "Şuan başka bir kanalda kullanılmaktayım!"));
                    return;
                }

            }
            catch (Exception ex)
            {
                await textChannel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(null, "Birşeyler Ters Gitti Üzgünüm :("));
            }

        }
    }
}
