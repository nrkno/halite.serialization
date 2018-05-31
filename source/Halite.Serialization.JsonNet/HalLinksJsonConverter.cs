using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Halite.Serialization.JsonNet
{
    public class HalLinksJsonConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new HalObjectDeserializer(objectType, serializer).Deserialize(reader);
        }

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

            var properties = objectType.GetInheritanceChain().Reverse().SelectMany(it => it.GetImmediateProperties()).ToList();
            foreach (var prop in properties.Where(p => p.CanRead))
            {
                var propVal = prop.GetValue(value, null);
                var name = prop.GetRelationName(serializer);

                if (propVal != null)
                {
                    try { 
                        jo.Add(name, JToken.FromObject(propVal, serializer));
                    }
                    catch (Exception ex)
                    {
                        throw new JsonWriterException($"Failed to add property with name {name} and value {propVal}!", ex);
                    }
                }
                else
                {
                    try
                    {
                        if (prop.CustomAttributes != null && prop.CustomAttributes.Any())
                        { 
                            var customJsonProperty = (JsonPropertyAttribute)prop.GetCustomAttribute(typeof(JsonPropertyAttribute));
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
                        throw new JsonWriterException($"Failed to add property with name {value} and value null!", ex);
                    }
                }
            }

            jo.WriteTo(writer);
        }


        public override bool CanConvert(Type objectType)
        {
            var result = typeof(HalLinks).IsAssignableFrom(objectType);
            return result;
        }
    }
}