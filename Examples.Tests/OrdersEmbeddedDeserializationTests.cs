namespace Halite.Serialization.Examples.Tests;

using System;
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