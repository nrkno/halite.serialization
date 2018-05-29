using Halite.Serialization.JsonNet;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Halite.Tests
{
    public class PropertyNameMapperFactoryTests
    {
        [Fact]
        public void PropertyNameIsMappedToHalRelationName()
        {
            var mapper = PropertyNameMapperFactory.Create(typeof(ExampleHalLinks));
            var name = mapper("thing");
            name.ShouldBe("acme:widget");
        }

        [Fact]
        public void PropertyNameIsMappedToJsonPropertyName()
        {
            var mapper = PropertyNameMapperFactory.Create(typeof(ExampleHalLinks));
            var name = mapper("foo");
            name.ShouldBe("bar");
        }

        [Fact]
        public void PropertyNameIsMappedToItself()
        {
            var mapper = PropertyNameMapperFactory.Create(typeof(ExampleHalLinks));
            var name = mapper("plain");
            name.ShouldBe("plain");
        }
    }

    internal class ExampleHalLinks : HalLinks
    {
        public ExampleHalLinks(SelfLink self, ThingLink thing, FooLink foo, PlainLink plain) : base(self)
        {
            Thing = thing;
            Foo = foo;
            Plain = plain;
        }

        [HalRelation("acme:widget")]
        public ThingLink Thing { get; }

        [JsonProperty("bar")]
        public FooLink Foo { get; }

        public PlainLink Plain { get; }
    }


    internal class ThingLink : HalLink
    {
        public ThingLink(string href) : base(href)
        {
        }
    }

    internal class FooLink : HalLink
    {
        public FooLink(string href) : base(href)
        {
        }
    }

    internal class PlainLink : HalLink
    {
        public PlainLink(string href) : base(href)
        {
        }
    }
}