using System.Collections.Generic;
using Halite.Serialization.JsonNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shouldly;
using Xunit;

namespace Halite.Examples.Tests
{
    public class OrdersResourceDeserializationTests
    {
        [Fact]
        public void VerifyOrdersResourceDeserialization()
        {
            var json = JsonTestFile.Read("OrdersResource.json");
            var resource = Deserialize<OrdersResource>(json);
            resource.ShouldNotBeNull();
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
}