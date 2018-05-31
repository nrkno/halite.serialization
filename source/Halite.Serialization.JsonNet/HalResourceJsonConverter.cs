using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Halite.Serialization.JsonNet
{
    public class HalResourceJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var objectType = value.GetType();

            try
            {
                DoWriteJson(writer, value, serializer, objectType);
            }
            catch (Exception ex)
            {
                throw new JsonWriterException($"Failed to serialize object of type {objectType}!", ex);
            }
        }

        private static void DoWriteJson(JsonWriter writer, object value, JsonSerializer serializer, Type objectType)
        {
            var jo = new JObject();

            var properties = objectType.GetInheritanceChain()
                .Reverse()
                .SelectMany(it => it.GetImmediateProperties())
                .ToList();

            var nameMapper = PropertyNameMapperFactory.Create(objectType);
            var niceJsonPropertyNames = properties.Select(p => nameMapper(p.Name)).ToList();

            var linksProperty = properties.Single(p => string.Equals("Links", p.Name, StringComparison.InvariantCultureIgnoreCase));
            var embeddedProperty = properties.SingleOrDefault(p => string.Equals("Embedded", p.Name, StringComparison.InvariantCultureIgnoreCase));
            var halResourceProperties = new[] {linksProperty, embeddedProperty}.Where(it => it != null);

            var regularProperties = properties.Except(halResourceProperties);

            AddPropertyValue(jo, "_links", linksProperty, value, serializer);

            foreach (var prop in regularProperties.Where(p => p.CanRead))
            {
                AddPropertyValue(jo, prop.GetPropertyName(serializer), prop, value, serializer);
            }

            if (embeddedProperty != null)
            {
                AddPropertyValue(jo, "_embedded", embeddedProperty, value, serializer);
            }

            jo.WriteTo(writer);
        }

        private static void AddPropertyValue(JObject jo, string name, PropertyInfo prop, object value, JsonSerializer serializer)
        {
            var propVal = prop.GetValue(value, null);

            if (propVal != null)
            {
                try
                {
                    jo.Add(name, JToken.FromObject(propVal, serializer));
                }
                catch (Exception ex)
                {
                    throw new JsonWriterException($"Failed to add property with name {name} and value {propVal}!", ex);
                }
            }
            else if (serializer.NullValueHandling == NullValueHandling.Include)
            {
                try
                {
                    if (prop.CustomAttributes != null && prop.CustomAttributes.Any())
                    {
                        var customJsonProperty =
                            (JsonPropertyAttribute) prop.GetCustomAttribute(typeof(JsonPropertyAttribute));
                        if (customJsonProperty == null) return;

                        var overriddenNullValueHandling = customJsonProperty.NullValueHandling;

                        if (!overriddenNullValueHandling.Equals(NullValueHandling.Ignore))
                        {
                            jo.Add(name, null);
                        }
                    }
                    else
                    {
                        jo.Add(name, null);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonWriterException($"Failed to add property with name {name} and value null!", ex);
                }
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new HalObjectDeserializer(objectType, serializer).Deserialize(reader);
        }

        public override bool CanConvert(Type objectType)
        {
            var res = IsHalResourceType(objectType);
            return res;
        }

        private static bool IsHalResourceType(Type type)
        {
            return type != null &&
                   (IsExactlyHalResourceType(type) || IsHalResourceType(type.BaseType));
        }

        private static bool IsExactlyHalResourceType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HalResource<>);
        }
    }
}