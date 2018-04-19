using Newtonsoft.Json.Serialization;

namespace Halite.Tests
{
    public class CustomSnakeCasingContractResolver : DefaultContractResolver
    {
        public CustomSnakeCasingContractResolver()
        {
            NamingStrategy = new SnakeCaseNamingStrategy();
        }
    }
}
