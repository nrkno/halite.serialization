# halite.serialization

This repository contains serialization projects for Halite.

## Halite.Serialization.JsonNet

This package enables serialization/deserialization support for Halite using Newtonsoft.Json. It is implemented as a custom contract resolver. You can use it like this:

```java
var settings = new JsonSerializerSettings 
{
    ContractResolver = new HalContractResolver()
}
```

`HalContractResolver` inherits from `DefaultContractResolver`, which means you can tune its behavior in all the usual ways. For instance, if you want to specify a `NamingStrategy`, you can do something like this:

```java
var settings = new JsonSerializerSettings 
{
    ContractResolver = new HalContractResolver 
    {
        NamingStrategy = new CamelCaseNamingStrategy
        {
            OverrideSpecifiedNames = false
        }
    }
}
```