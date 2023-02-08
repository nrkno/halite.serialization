namespace Halite.Serialization.JsonNet;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class HalContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var jp = base.CreateProperty(member, memberSerialization);

        var name = member.GetCustomAttribute<HalRelationAttribute>()?.Name ??
                    member.GetCustomAttribute<HalPropertyAttribute>()?.Name;

        var prop = (PropertyInfo) member;

        if (name != null)
        {
            jp.PropertyName = name;
        }

        var isHalLinkProperty = typeof(HalLinkObject).IsAssignableFrom(member.DeclaringType);
        if (isHalLinkProperty)
        {
            jp.ShouldSerialize =
                it =>
                {
                    var val = prop.GetMethod.Invoke(it, null);
                    return val != null;
                };
        }

        return jp;
    }

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var properties = base.CreateProperties(type, memberSerialization);

        var isHalLinkType = typeof(HalLinkObject).IsAssignableFrom(type);
        if (isHalLinkType)
        {
            return EnumerateHalLinkProperties(properties).ToList();
        }

        var isHalLinksType = typeof(HalLinks).IsAssignableFrom(type);
        if (isHalLinksType)
        {
            return EnumerateHalLinksProperties(properties).ToList();
        }

        var isHalResourceType = IsHalResourceType(type);
        if (isHalResourceType)
        {
            return EnumerateHalResourceProperties(properties).ToList();
        }

        return properties;
    }

    private static IEnumerable<JsonProperty> EnumerateHalLinkProperties(IList<JsonProperty> properties)
    {
        var dict = properties.ToDictionary(it => it.PropertyName);
        yield return dict["name"];
        yield return dict["href"];
        yield return dict["templated"];
        yield return dict["type"];
        yield return dict["deprecation"];
        yield return dict["profile"];
        yield return dict["title"];
        yield return dict["hreflang"];
    }

    private static IEnumerable<JsonProperty> EnumerateHalLinksProperties(IList<JsonProperty> properties)
    {
        var selfProperty = properties.Single(p => string.Equals("self", p.PropertyName, StringComparison.InvariantCultureIgnoreCase));
        yield return selfProperty;
        foreach (var p in properties.Except(new[] {selfProperty}))
        {
            yield return p;
        }
    }

    private static IEnumerable<JsonProperty> EnumerateHalResourceProperties(IList<JsonProperty> properties)
    {
        var linksProperty = properties.SingleOrDefault(p => string.Equals("_links", p.PropertyName, StringComparison.InvariantCultureIgnoreCase));
        var embeddedProperty = properties.SingleOrDefault(p => string.Equals("_embedded", p.PropertyName, StringComparison.InvariantCultureIgnoreCase));
        var halResourceProperties = new[] { linksProperty, embeddedProperty }.Where(it => it != null);
        var regularProperties = properties.Except(halResourceProperties);

        if (linksProperty != null) yield return linksProperty;

        foreach (var p in regularProperties)
        {
            yield return p;
        }

        if (embeddedProperty != null) yield return embeddedProperty;
    }

    protected override JsonObjectContract CreateObjectContract(Type objectType)
    {
        var c = base.CreateObjectContract(objectType);
        if (typeof(HalLinkObject).IsAssignableFrom(objectType))
        {
            return ModifyObjectContractForHalLinkObject(objectType, c);
        }

        return c;
    }

    private JsonObjectContract ModifyObjectContractForHalLinkObject(Type objectType, JsonObjectContract c)
    {
        var ctor = SelectConstructor(objectType);
        if (ctor != null)
        {
            c.OverrideCreator = CreateParameterizedConstructor(ctor);
            c.CreatorParameters.Clear();
            var cps = CreateConstructorParameters(ctor, c.Properties);
            foreach (var cp in cps)
            {
                c.CreatorParameters.Add(cp);
            }
        }

        return c;
    }

    private ObjectConstructor<object> CreateParameterizedConstructor(MethodBase method)
    {
        if (method == null) throw new ArgumentNullException(nameof(method));

        var c = method as ConstructorInfo;
        if (c != null)
        {
            return a =>
            {
                try
                {
                    return c.Invoke(a);
                }
                catch (TargetInvocationException ex)
                {
                    if (ex.InnerException == null) throw;
                    throw new JsonSerializationException(ex.InnerException.Message, ex.InnerException);
                }
            };
        }

        return a => method.Invoke(null, a);
    }

    private static ConstructorInfo SelectConstructor(Type objectType)
    {
        return SelectHalLinkConstructor(objectType) ??
                SelectSubclassConstructor(objectType);
    }

    private static ConstructorInfo SelectHalLinkConstructor(Type objectType)
    {
        if (objectType == typeof(HalLink) || objectType == typeof(HalTemplatedLink))
        {
            return objectType.GetConstructors().Single(AcceptsSingleStringParameter);
        }

        return null;
    }

    private static bool AcceptsSingleStringParameter(ConstructorInfo ctor)
    {
        var parameters = ctor.GetParameters();
        return parameters.Length == 1 && parameters[0].ParameterType == typeof(string);
    }

    private static ConstructorInfo SelectSubclassConstructor(Type objectType)
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