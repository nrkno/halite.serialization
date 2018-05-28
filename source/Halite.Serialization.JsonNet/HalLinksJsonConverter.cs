using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Halite.Serialization.JsonNet
{
    public class HalLinksJsonConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return JsonReader<HalLinks>.Read(reader, objectType, (HalLinks)existingValue, serializer);
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
                if (propVal != null)
                {
                    jo.Add(prop.GetRelationName(serializer), JToken.FromObject(propVal, serializer));
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