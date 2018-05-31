using System;
using Halite.Serialization.JsonNet;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Halite.Tests
{
    public class HalLinksDeserializationTests
    {
        [Fact]
        public void DeserializeNull()
        {
            Deserialize<HalLinks>("null").ShouldBeNull();
        }

        [Fact]
        public void VerifyMinimalHalLinksDeserialization()
        {
            const string json = "{\"self\":{\"href\":\"/things/1\"}}";
            HalLinks links = Deserialize<HalLinks>(json);
            var self = links.Self;
            self.Href.ToString().ShouldBe("/things/1");
            self.Templated.ShouldBeNull();
        }

        [Fact]
        public void VerifyMinimalHalLinksDeserializationFailsWithoutSelf()
        {
            const string json = "{\"this\":{\"href\":\"/things/1\"}}";
            Assert.Throws<ArgumentNullException>(() => Deserialize<DummyLinks>(json));
        }

        [Fact]
        public void VerifyDummyLinksDeserializationWithNulls()
        {
            const string json = "{\"self\":{\"href\":\"/things/1\"}}";
            DummyLinks links = Deserialize<DummyLinks>(json);
            var selfLink = links.Self;
            selfLink.Href.ToString().ShouldBe("/things/1");
            selfLink.Templated.ShouldBeNull();
            links.This.ShouldBeNull();
            links.That.ShouldBeNull();
        }

        [Fact]
        public void VerifyDummyLinksDeserializationWithThis()
        {
            const string json = "{\"self\":{\"href\":\"/things/1\"},\"this\":{\"href\":\"/this\"}}";
            var links = Deserialize<DummyLinksWithNotNullThis>(json);
            var selfLink = links.Self;
            selfLink.Href.ToString().ShouldBe("/things/1");
            selfLink.Templated.ShouldBeNull();
            var thisLink = links.This;
            thisLink.Href.ToString().ShouldBe("/this");
            links.That.ShouldBeNull();
        }

        [Fact]
        public void VerifyDummyLinksDeserializationWithPrivateConstructor()
        {
            const string json = "{\"self\":{\"href\":\"/things/1\"}}";
            DummyLinksWithPrivateConstructor links = Deserialize<DummyLinksWithPrivateConstructor>(json);
            var selfLink = links.Self;
            selfLink.Href.ToString().ShouldBe("/things/1");
            selfLink.Templated.ShouldBeNull();
            links.This.ShouldBeNull();
            links.That.ShouldBeNull();
        }

        [Fact]
        public void VerifyDummyLinksDeserializationWithLinks()
        {
            const string json = "{\"self\":{\"href\":\"/things/1\"},\"this\":{\"href\":\"/this\"},\"that\":{\"href\":\"/that\"}}";
            DummyLinks links = Deserialize<DummyLinks>(json);
            var selfLink = links.Self;
            selfLink.Href.ToString().ShouldBe("/things/1");
            selfLink.Templated.ShouldBeNull();
            links.This.Href.ToString().ShouldBe("/this");
            links.That.Href.ToString().ShouldBe("/that");
        }

        [Fact]
        public void VerifyDtoLinksDeserialization()
        {
            const string json = "{\"self\":{\"href\":\"/things/1\"},\"link1\":{\"href\":\"/link/1\"},\"link2\":{\"href\":\"/link/2\"}}";
            var links = Deserialize<DtoLinks>(json);
            var selfLink = links.Self;
            selfLink.Href.ToString().ShouldBe("/things/1");
            selfLink.Templated.ShouldBeNull();
            links.Link1.ShouldNotBeNull();
            links.Link2.ShouldNotBeNull();
        }

        [Fact]
        public void VerifyDtoLinksDeserializationWithNull()
        {
            const string json = "{\"self\":{\"href\":\"/things/1\"},\"link1\":null}";
            var links = Deserialize<DtoLinks>(json);
            var selfLink = links.Self;
            selfLink.Href.ToString().ShouldBe("/things/1");
            selfLink.Templated.ShouldBeNull();
            links.Link1.ShouldBeNull();
            links.Link2.ShouldBeNull();
        }

        [Fact]
        public void VerifyHalLinksWithJsonPropertyThroughConstructor()
        {
            string json =
                @"{
    ""self"": {
      ""href"": ""/me/myself/i""
    },
    ""ad:hoc"": {
      ""href"": ""/some/ad/hoc/link""
    }
}";
            var links = Deserialize<AdhocHalLinks>(json);
            links.Self.Href.ToString().ShouldBe("/me/myself/i");
            links.AdhocLink.Href.ToString().ShouldBe("/some/ad/hoc/link");
        }

        [Fact]
        public void VerifyHalLinksWithJsonPropertyThroughSetter()
        {
            string json =
                @"{
    ""self"": {
      ""href"": ""/icecream""
    },
    ""fizz:buzz"": {
      ""href"": ""/1/2/fizz/4/buzz""
    }
}";
            var links = Deserialize<FizzBuzzHalLinks>(json);
            links.Self.Href.ToString().ShouldBe("/icecream");
            links.FizzBuzzLink.ShouldNotBeNull();
            links.FizzBuzzLink.Href.ToString().ShouldBe("/1/2/fizz/4/buzz");
        }

        [Fact]
        public void VerifyDummyLinksDeserializationWithNullValueHandlingPropertySet()
        {
            const string json = "{\"self\":{\"href\":\"/things/1\"},\"this\":{\"href\":\"/this\"}}";
            var links = Deserialize<DummyLinksWithNullValueHandling>(json);
            var selfLink = links.Self;
            selfLink.Href.ToString().ShouldBe("/things/1");
            selfLink.Templated.ShouldBeNull();
            var thisLink = links.This;
            thisLink.Href.ToString().ShouldBe("/this");
            links.That.ShouldBeNull();
        }

        private static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings().ConfigureForHalite());
        }
    }

    internal class AdhocHalLinks : HalLinks
    {
        public AdhocHalLinks(SelfLink self, HalLink adhocLink) : base(self)
        {
            AdhocLink = adhocLink;
        }

        [JsonProperty(PropertyName = "ad:hoc")]
        public HalLink AdhocLink { get; }
    }

    internal class FizzBuzzHalLinks : HalLinks
    {
        public FizzBuzzHalLinks(SelfLink self) : base(self)
        {
        }

        [JsonProperty(PropertyName = "fizz:buzz")]
        public HalLink FizzBuzzLink { get; set; }
    }
}