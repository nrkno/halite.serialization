namespace Halite.Tests;

using Newtonsoft.Json.Serialization;

public class CustomSnakeCasingContractResolver : DefaultContractResolver
{
    public CustomSnakeCasingContractResolver()
    {
        NamingStrategy = new SnakeCaseNamingStrategy();
    }
}