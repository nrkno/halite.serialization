namespace Halite.Serialization.JsonNet.Tests;

using Newtonsoft.Json;

public class SnakeResource : HalResource<HalLinks>
{
    public string LongTail { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; }
}