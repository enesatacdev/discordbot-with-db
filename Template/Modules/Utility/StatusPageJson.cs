using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Modules.Utility
{
    public class StatusPagePageJson
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("url")]
        public string Url;

        [JsonProperty("time_zone")]
        public string TimeZone;

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt;
    }

    public class StatusPageComponentJson
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("status")]
        public string Status;

        [JsonProperty("created_at")]
        public DateTime CreatedAt;

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt;

        [JsonProperty("position")]
        public int Position;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("showcase")]
        public bool Showcase;

        [JsonProperty("start_date")]
        public string StartDate;

        [JsonProperty("group_id")]
        public string GroupId;

        [JsonProperty("page_id")]
        public string PageId;

        [JsonProperty("group")]
        public bool Group;

        [JsonProperty("only_show_if_degraded")]
        public bool OnlyShowIfDegraded;

        [JsonProperty("components")]
        public List<string> Components;
    }

    public class StatusPageStatusJson
    {
        [JsonProperty("indicator")]
        public string Indicator;

        [JsonProperty("description")]
        public string Description;
    }

    public class StatusPageJson
    {
        [JsonProperty("page")]
        public StatusPagePageJson Page;

        [JsonProperty("components")]
        public List<StatusPageComponentJson> Components;

        [JsonProperty("status")]
        public StatusPageStatusJson Status;
    }
}
