﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Modules.Search
{
    public class TraceMoeDocJson
    {
        [JsonProperty("from")]
        public double? From { get; set; }

        [JsonProperty("to")]
        public double? To { get; set; }

        [JsonProperty("anilist_id")]
        public int? AnilistId { get; set; }

        [JsonProperty("at")]
        public double? At { get; set; }

        [JsonProperty("season")]
        public string Season { get; set; }

        [JsonProperty("anime")]
        public string Anime { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("episode")]
        public double? Episode { get; set; }

        [JsonProperty("tokenthumb")]
        public string Tokenthumb { get; set; }

        [JsonProperty("similarity")]
        public double? Similarity { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("title_native")]
        public string TitleNative { get; set; }

        [JsonProperty("title_chinese")]
        public string TitleChinese { get; set; }

        [JsonProperty("title_english")]
        public string TitleEnglish { get; set; }

        [JsonProperty("title_romaji")]
        public string TitleRomaji { get; set; }

        [JsonProperty("mal_id")]
        public int? MalId { get; set; }

        [JsonProperty("synonyms")]
        public List<string> Synonyms { get; set; }

        [JsonProperty("synonyms_chinese")]
        public List<string> SynonymsChinese { get; set; }

        [JsonProperty("is_adult")]
        public bool IsAdult { get; set; }

        [JsonIgnore]
        public string ImageThumbnail => $"https://trace.moe/thumbnail.php?anilist_id={AnilistId}&file={Uri.EscapeDataString(Filename)}&t={At}&token={Tokenthumb}";

        [JsonIgnore]
        public string VideoThumbnail => $"https://media.trace.moe/video/{AnilistId}/{Uri.EscapeDataString(Filename)}?t={At}&token={Tokenthumb}`";
    }

    public class TraceMoeJson
    {
        [JsonProperty("RawDocsCount")]
        public int RawDocsCount { get; set; }

        [JsonProperty("RawDocsSearchTime")]
        public long RawDocsSearchTime { get; set; }

        [JsonProperty("ReRankSearchTime")]
        public long ReRankSearchTime { get; set; }

        [JsonProperty("CacheHit")]
        public bool CacheHit { get; set; }

        [JsonProperty("trial")]
        public int Trial { get; set; }

        [JsonProperty("docs")]
        public List<TraceMoeDocJson> Docs { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("limit_ttl")]
        public int LimitTtl { get; set; }

        [JsonProperty("quota")]
        public int Quota { get; set; }

        [JsonProperty("quota_ttl")]
        public int QuotaTtl { get; set; }
    }
}
