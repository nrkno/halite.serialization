namespace Halite.Serialization.Examples.Tests;

using System;
using Halite.Serialization.JsonNet;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

public class OrderLineLinksDeserializationTests
{
    [Fact]
    public void VerifyDeserialization()
    {
        var json = "{\"self\":{\"href\":\"/orders/123\"},\"ea:basket\":{\"href\":\"/baskets/98712\"},\"ea:customer\":{\"href\":\"/customers/7809\"}}";
        var links = Deserialize<OrderLineLinks>(json);
        links.Self.Href.ToString().ShouldBe("/orders/123");
        links.BasketLink.Href.ToString().ShouldBe("/baskets/98712");
        links.CustomerLink.Href.ToString().ShouldBe("/customers/7809");
    }

    private static T Deserialize<T>(string json)
    {
        var settings = new JsonSerializerSettings().ConfigureForHalite();
        var deserialized = JsonConvert.DeserializeObject<T>(json, settings);
        if (deserialized == null)
        {
            throw new NullReferenceException($"{nameof(JsonConvert.DeserializeObject)} returned null for JSON: {json}");
        }
        else
        {
            return deserialized;
        }
    }
}