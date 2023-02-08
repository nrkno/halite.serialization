namespace Halite.Tests;

using System.Collections.Generic;
using Halite.Serialization.JsonNet;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

public class HalLinksSerializationTests
{
    [Fact]
    public void VerifyBasicLinkSerialization()
    {
        var dummy = new DummyLinksWithNotNullThis(new SelfLink("/self"), new ThisLink(), new ThatLink(), new List<HalLink>());

        var settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Include
        };

        settings.ConfigureForHalite();

        var result = JsonConvert.SerializeObject(dummy, settings);
        result.ShouldBe("{\"self\":{\"href\":\"/self\"},\"this\":{\"href\":\"/this\"},\"that\":{\"href\":\"/that\"},\"those\":[]}");
    }

    [Fact]
    public void VerifyNullValueLinkSerialization()
    {
        var dummy = new DummyLinksWithNullValueHandling(new SelfLink("/self"), new ThisLink(), null, new List<HalLink>());

        var settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Include
        };

        settings.ConfigureForHalite();

        var result = JsonConvert.SerializeObject(dummy, settings);
        result.ShouldBe("{\"self\":{\"href\":\"/self\"},\"this\":{\"href\":\"/this\"},\"that\":null,\"those\":[]}");
    }
}