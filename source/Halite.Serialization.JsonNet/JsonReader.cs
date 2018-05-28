using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Halite.Serialization.JsonNet
{
    public class JsonReader<T>
    {
        public static T Read(JsonReader reader, Type objectType, T existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var jo = JObject.Load(reader);

                var ctor = SelectConstructor(objectType);
                if (ctor == null)
                {
                    throw CreateConstructorException(objectType);
                }

                var instance = CreateInstance(objectType, ctor, jo, serializer);

                AssignValues(objectType, instance, jo);

                return instance;
            }

            throw new InvalidOperationException();
        }

        private static void AssignValues(Type objectType, object instance, JObject jo)
        {
            var properties = objectType.GetProperties().Where(p => p.SetMethod != null && p.GetMethod != null).ToList();

            foreach (var prop in properties)
            {
                var jop = jo.Properties().FirstOrDefault(it =>
                    string.Equals(it.Name, prop.Name, StringComparison.InvariantCultureIgnoreCase));
                if (jop != null)
                {
                    var currentValue = prop.GetMethod.Invoke(instance, new object[0]);
                    if (currentValue == null)
                    {
                        var jvalue = (JValue)jop.Value;
                        var objValue = jvalue.Value;
                        var value = typeof(Uri) == prop.PropertyType
                            ? new Uri((string)objValue, UriKind.RelativeOrAbsolute)
                            : objValue;
                        prop.SetMethod.Invoke(instance, new[] { value });
                    }
                }
            }
        }

        private static object CreateArgument(JProperty maybeProperty, Type parameterType, JsonSerializer serializer)
        {
            return maybeProperty == null
                ? parameterType.GetDefaultValue()
                : maybeProperty.Value.ToObject(parameterType, serializer);
        }

        private static T CreateInstance(Type objectType, ConstructorInfo ctor, JObject item, JsonSerializer serializer)
        {
            var nameMapper = PropertyNameMapperFactory.Create(objectType);
            Func<string, JProperty> lookupProperty = n => item.Properties().FirstOrDefault(jp =>
                string.Equals(n, jp.Name, StringComparison.InvariantCultureIgnoreCase));
            var read = nameMapper.Compose(lookupProperty);

            var args = ctor.GetParameters().Select(p => CreateArgument(read(p.Name), p.ParameterType, serializer)).ToArray();

            try
            {
                return (T)ctor.Invoke(args);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        private static JsonSerializationException CreateConstructorException(Type objectType)
        {
            return new JsonSerializationException($"Unable to find a constructor to use for type {objectType}. A class should either have a default constructor, one constructor with arguments or a constructor marked with the JsonConstructor attribute.");
        }

        private static ConstructorInfo SelectConstructor(Type objectType)
        {
            var constructors = objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            return SelectAnnotatedJsonConstructor(constructors) ??
                   SelectDefaultConstructor(constructors) ??
                   SelectConstructorWithParameters(constructors);
        }

        private static ConstructorInfo SelectAnnotatedJsonConstructor(IReadOnlyList<ConstructorInfo> constructors)
        {
            return constructors.SingleOrDefault(ctor => ctor.GetCustomAttributes(typeof(JsonConstructorAttribute), false).Any());
        }

        private static ConstructorInfo SelectDefaultConstructor(IReadOnlyList<ConstructorInfo> constructors)
        {
            return constructors.SingleOrDefault(ctor => !ctor.GetParameters().Any());
        }

        private static ConstructorInfo SelectConstructorWithParameters(IReadOnlyList<ConstructorInfo> ctors)
        {
            return ctors.Count == 1 ? ctors[0] : null;
        }

    }
}