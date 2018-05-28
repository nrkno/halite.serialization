using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Halite.Serialization.JsonNet
{
    public static class PropertyNameMapperFactory
    {
        private static string GetOverridingPropertyName(PropertyInfo p)
        {
            return p.GetCustomAttribute<HalRelationAttribute>()?.Name ??
                   p.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName;
        }

        public static Func<string, string> Create(Type objectType)
        {
            var props = objectType.GetProperties();

            Func<string, string> id = Functions.Identity;
            Func<Func<string, string>, PropertyInfo, Func<string, string>> combiner = (fn, p) =>
            {
                var overridingName = GetOverridingPropertyName(p);
                if (overridingName != null)
                {
                    return n => string.Equals(n, p.Name, StringComparison.InvariantCultureIgnoreCase)
                        ? overridingName
                        : fn(n);
                }

                return fn;
            };

            return props.Aggregate(id, combiner);
        }
    }
}
