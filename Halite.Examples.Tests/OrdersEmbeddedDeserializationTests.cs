namespace Halite.Examples.Tests;

using Halite.Serialization.JsonNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shouldly;
using Xunit;

public class OrdersEmbeddedDeserializationTests
{
    [Fact]
    public void VerifyOrdersResource()
    {
        var json = JsonTestFile.OrdersEmbedded;
        var embedded = Deserialize<OrdersEmbedded>(json);
        embedded.ShouldNotBeNull();
    }

    private static T Deserialize<T>(string json)
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new HalContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };

        return JsonConvert.DeserializeObject<T>(json, settings);
    }
}