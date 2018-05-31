using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Halite.Serialization.JsonNet
{
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
}
