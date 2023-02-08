namespace Halite.Serialization.JsonNet;

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public static class SerializerConfigExtensions
{
    public static JsonSerializerSettings ConfigureForHalite(this JsonSerializerSettings settings)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        settings.ContractResolver = new HalContractResolver();

        return settings;
    }

    public static JsonSerializer ConfigureForHalite(this JsonSerializer serializer)
    {
        if (serializer == null)
        {
            throw new ArgumentNullException(nameof(serializer));
        }

        serializer.ContractResolver = new HalContractResolver();

        return serializer;
    }
}