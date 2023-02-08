namespace Halite.Serialization.JsonNet.Tests;

using System.Linq;
using Halite.Serialization.JsonNet;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

public class HalResourceDeserializationTests
{
    [Fact]
    public void DeserializeNull()
    {
        Deserialize<TurtleResource>("null").ShouldBeNull();
    }

    [Fact]
    public void VerifyDeserializeDummyResourceWithLinks()
    {
        const string json = "{\"_links\":{\"this\":{\"href\":\"/this\"},\"that\":{\"href\":\"/that\"},\"self\":{\"href\":\"/lambda\"},\"those\":[{\"href\":\"/quux\"},{\"href\":\"/xuuq\"}]}}";
        var resource = Deserialize<DummyResourceWithLinks>(json);
        resource.Links.ShouldNotBeNull();
        resource.Links.Self.Href.ToString().ShouldBe("/lambda");
    }

    [Fact]
    public void VerifyDeserializeDummyResourceWithPrivateConstructor()
    {
        const string json = "{\"_links\":{\"this\":{\"href\":\"/this\"},\"that\":{\"href\":\"/that\"},\"self\":{\"href\":\"/lambda\"},\"those\":[{\"href\":\"/quux\"},{\"href\":\"/xuuq\"}]}}";
        var resource = Deserialize<DummyResourceWithPrivateConstructor>(json);
        resource.Links.ShouldNotBeNull();
        resource.Links.Self.Href.ToString().ShouldBe("/lambda");
    }

    [Fact]
    public void VerifyDeserializeDummyResourceWithLinksAndNullEmbedded()
    {
        const string json = "{\"_links\":{\"this\":{\"href\":\"/this\"},\"that\":{\"href\":\"/that\"},\"self\":{\"href\":\"/lambda\"},\"those\":[{\"href\":\"/quux\"},{\"href\":\"/xuuq\"}]}}";
        var resource = Deserialize<DummyResourceWithLinksAndEmbedded>(json);
        resource.Links.Self.Href.ToString().ShouldBe("/lambda");
        resource.Embedded.ShouldBeNull();
    }

    [Fact]
    public void VerifyDeserializeDummyResourceWithLinksAndEmbedded()
    {
        const string json = "{\"_links\":{\"this\":{\"href\":\"/this\"},\"that\":{\"href\":\"/that\"},\"self\":{\"href\":\"/lambda\"},\"those\":[{\"href\":\"/quux\"},{\"href\":\"/xuuq\"}]},\"_embedded\":{}}";
        var resource = Deserialize<DummyResourceWithLinksAndEmbedded>(json);
        resource.Links.Self.Href.ToString().ShouldBe("/lambda");
        resource.Embedded.ShouldNotBeNull();
    }

    [Fact]
    public void VerifyDeserializeTurtleResource()
    {
        const string json =
            "{\"_links\":{\"self\":{\"href\":\"/turtle2\"}},\"_embedded\":{\"Down\":{\"_links\":{\"self\":{\"href\":\"/turtle1\"}},\"_embedded\":{\"Down\":{\"_links\":{\"self\":{\"href\":\"/turtle0\"}}}}}}}";
        var turtle = Deserialize<TurtleResource>(json);
        VerifyTurtles(turtle, "/turtle2", "/turtle1", "/turtle0");
    }

    [Fact]
    public void VerifyMinimalHumanResource()
    {
        string json =
            @"{
""_links"": {
    ""self"": {
        ""href"": ""/resources/human/6""
    }
}
}";
        var hr = Deserialize<HumanResource>(json);
        hr.Links.ShouldNotBeNull();
        var self = hr.Links.Self;
        self.ShouldNotBeNull();
        self.Href.ToString().ShouldBe("/resources/human/6");
        hr.Age.ShouldBe(default(int));
        hr.Name.ShouldBeNull();
    }

    [Fact]
    public void VerifyHumanResourceWithAge()
    {
        string json =
            @"{
""_links"": {
    ""self"": {
        ""href"": ""/resources/human/6""
    }
},
""Age"": 35
}";
        var hr = Deserialize<HumanResource>(json);
        hr.Links.ShouldNotBeNull();
        var self = hr.Links.Self;
        self.ShouldNotBeNull();
        self.Href.ToString().ShouldBe("/resources/human/6");
        hr.Age.ShouldBe(35);
        hr.Name.ShouldBeNull();
    }

    [Fact]
    public void VerifyHumanResourceWithName()
    {
        string json =
            @"{
""_links"": {
    ""self"": {
        ""href"": ""/resources/human/6""
    }
},
""hr:name"": ""Peter Smith""
}";
        var hr = Deserialize<HumanResource>(json);
        hr.Links.ShouldNotBeNull();
        var self = hr.Links.Self;
        self.ShouldNotBeNull();
        self.Href.ToString().ShouldBe("/resources/human/6");
        hr.Age.ShouldBe(default(int));
        hr.Name.ShouldBe("Peter Smith");
    }

    private static T Deserialize<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings().ConfigureForHalite());
    }

    private static void VerifyTurtles(TurtleResource turtle, params string[] expectedLinks)
    {
        foreach (var pair in turtle.AllTheWayDown().Select(t => t.Links.Self.Href.ToString())
            .Zip(expectedLinks, (actual, expected) => new { actual, expected }))
        {
            pair.actual.ShouldBe(pair.expected);
        }
    }
}