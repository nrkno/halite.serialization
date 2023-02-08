namespace Halite.Serialization.JsonNet.Tests;

using Newtonsoft.Json.Serialization;

public class CustomSnakeCasingContractResolver : DefaultContractResolver
{
    public CustomSnakeCasingContractResolver()
    {
        NamingStrategy = new SnakeCaseNamingStrategy();
    }
}