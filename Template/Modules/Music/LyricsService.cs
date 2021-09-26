using Genius;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Template.Modules.Music
{
    public static class LyricsService
    {
        private static readonly GeniusClient client = new GeniusClient("PXiIHw75McNRp1c2d6tIn5083AwxL-gAAjXcdg0A3k0HaLWOi6uwsUNmsGgbygQQ");

        private static readonly Lazy<HttpClient> LazyHttpClient = new Lazy<HttpClient>();
        internal static readonly HttpClient HttpClient = LazyHttpClient.Value;

        public static string Title { get; private set; }
        public static string TrackURL { get; private set; }
        public static string TrackImage { get; private set; }

        public static async ValueTask<string> GetLyricsFromGenius(string query)
            => await SearchGeniusAsync(query);

        public static async ValueTask<string> SearchGeniusAsync(string query)
        {
            Title = null; TrackURL = null; TrackImage = null;
            var result = client.SearchClient.Search(query).Result;

            if (result.Response.Hits.Count == 0)
                return "Lyrics Bulunamadı";

            var track = result.Response.Hits.FirstOrDefault().Result;
            

            Title = track.FullTitle;
            TrackURL = track.Url;
            TrackImage = track.HeaderImageThumbnailUrl; 

            var responseMessage = await HttpClient.GetAsync(track.Url);
            using var content = responseMessage.Content;

            var responseData = await content.ReadAsByteArrayAsync();

            string ParseGeniusHtml()
            {
                var start = Encoding.UTF8.GetBytes("<!--sse-->");
                var end = Encoding.UTF8.GetBytes("<!--/sse-->");

                Span<byte> bytes = responseData;
                bytes = bytes[bytes.LastIndexOf(start)..];
                bytes = bytes[..bytes.LastIndexOf(end)];

                var rawHtml = Encoding.UTF8.GetString(bytes);
                if (rawHtml.Contains("Genius.ads"))
                {
                    return string.Empty;
                }

                var htmlRegex = new Regex("<[^>]*?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                return htmlRegex.Replace(rawHtml, string.Empty).TrimStart().TrimEnd();
            }

            return ParseGeniusHtml();
        }
    }
}
