using Newtonsoft.Json;

namespace Halite.Tests
{
    public class SnakeResource : HalResource<HalLinks>
    {
        public string LongTail { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }
}
