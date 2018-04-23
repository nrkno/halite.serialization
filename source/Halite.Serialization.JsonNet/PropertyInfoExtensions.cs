using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Halite.Serialization.JsonNet
{
    internal static class PropertyInfoExtensions
    {
        public static string GetRelationName(this PropertyInfo prop, JsonSerializer serializer)
        {
            var attribute = prop.GetCustomAttribute(typeof(HalRelationAttribute)) as HalRelationAttribute;
            return attribute == null ? GetResolvedPropertyName(prop.Name, serializer) : attribute.Name;
        }

        public static string GetPropertyName(this PropertyInfo prop, JsonSerializer serializer)
        {
            var attribute = prop.GetCustomAttribute(typeof(HalPropertyAttribute)) as HalPropertyAttribute;
            return attribute == null ? GetResolvedPropertyName(prop.Name, serializer) : attribute.Name;
        }

        private static string GetResolvedPropertyName(string propertyName, JsonSerializer serializer)
        {
            if (serializer.ContractResolver is DefaultContractResolver contractResolver)
            {
                return contractResolver.GetResolvedPropertyName(propertyName);
            }

            return propertyName;
        }
    }
}