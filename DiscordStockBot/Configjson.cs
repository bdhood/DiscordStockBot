using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DiscordStockBot
{
    public struct Configjson
    {
        [JsonProperty("prefixes")]
        public string[] Prefix { get; private set; }    
    }
}
