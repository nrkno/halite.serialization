using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Halite.Serialization.JsonNet
{
    public class HalObjectDeserializer
    {
        private readonly Type _objectType;
        private readonly JsonSerializer _serializer;
        private readonly Func<string, string> _toJsonName;

        public HalObjectDeserializer(Type objectType, JsonSerializer serializer)
        {
            _objectType = objectType;
            _serializer = serializer;
            _toJsonName = PropertyNameMapperFactory.Create(objectType);
        }

        public object Deserialize(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var jo = JObject.Load(reader);
                return Deserialize(jo);
            }
            else if (reader.TokenType == JsonToken.Null)
            {
                return _objectType.GetDefaultValue();
            }

            throw new InvalidOperationException();
        }

        private object Deserialize(JObject jo)
        {
            var ctor = SelectConstructor();
            if (ctor == null)
            {
                throw CreateConstructorException();
            }

            var instance = CreateInstance(ctor, jo);

            AssignValues(instance, jo);

            return instance;
        }

        private void AssignValues(object instance, JObject jo)
        {
            var properties = _objectType.GetProperties().Where(p => p.SetMethod != null && p.GetMethod != null).ToList();

            foreach (var prop in properties)
            {
                var jsonPropName = _toJsonName(prop.Name);
                var jop = jo.Properties().FirstOrDefault(it =>
                    string.Equals(it.Name, jsonPropName, StringComparison.InvariantCultureIgnoreCase));

                if (jop != null)
                {
                    var currentValue = prop.GetMethod.Invoke(instance, new object[0]);
                    var defaultValue = prop.PropertyType.GetDefaultValue();
                    if (Equals(currentValue, defaultValue))
                    {
                        var value = jop.Value.ToObject(prop.PropertyType, _serializer);
                        prop.SetMethod.Invoke(instance, new[] { value });
                    }
                }
            }
        }

        private object CreateArgument(JProperty maybeProperty, Type parameterType)
        {
            return maybeProperty == null
                ? parameterType.GetDefaultValue()
                : maybeProperty.Value.ToObject(parameterType, _serializer);
        }

        private object CreateInstance(ConstructorInfo ctor, JObject item)
        {
            Func<string, JProperty> lookupJsonProperty = n => item.Properties().FirstOrDefault(jp =>
                string.Equals(n, jp.Name, StringComparison.InvariantCultureIgnoreCase));
            var read = _toJsonName.Compose(lookupJsonProperty);

            var args = ctor.GetParameters().Select(p => CreateArgument(read(p.Name), p.ParameterType)).ToArray();

            try
            {
                return ctor.Invoke(args);
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

        private JsonSerializationException CreateConstructorException()
        {
            return new JsonSerializationException($"Unable to find a constructor to use for type {_objectType}. A class should either have a default constructor, one constructor with arguments or a constructor marked with the JsonConstructor attribute.");
        }

        private ConstructorInfo SelectConstructor()
        {
            var constructors = _objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

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