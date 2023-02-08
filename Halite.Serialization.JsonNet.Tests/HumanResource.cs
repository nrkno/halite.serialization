namespace Halite.Serialization.JsonNet.Tests;

using Newtonsoft.Json;

internal class HumanResource : HalResource<HumanLinks>
{
    public HumanResource(string name)
    {
        Name = name;
    }

    [JsonProperty(PropertyName = "hr:name")]
    public string Name { get; }

    public int Age { get; set; }
}

internal class HumanLinks : HalLinks
{
    public HumanLinks(SelfLink self) : base(self)
    {
    }

    public HalLink Next { get; set; }
}