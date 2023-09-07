using Newtonsoft.Json;

namespace Abyss_Bot
{
    public struct Config
    {
        [JsonProperty("Token")]
        public string Token { get; set; }
        [JsonProperty("Prefix")]
        public string Prefix { get; set; }
    }
}
