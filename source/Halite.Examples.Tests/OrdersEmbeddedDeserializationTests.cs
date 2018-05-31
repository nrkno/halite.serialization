using Halite.Serialization.JsonNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shouldly;
using Xunit;

namespace Halite.Examples.Tests
{
    public class OrdersEmbeddedDeserializationTests
    {
        [Fact]
        public void VerifyOrdersResource()
        {
            var json = JsonTestFile.Read("OrdersEmbedded.json");
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
}