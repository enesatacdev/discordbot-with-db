using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Modules.Search
{
    public class MinecraftProfileJson
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string ID { get; set; }
    }

    public class MinecraftNameHistoryJson
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("changedToAt")]
        public long? ChangedToAt { get; set; }
    }
}
