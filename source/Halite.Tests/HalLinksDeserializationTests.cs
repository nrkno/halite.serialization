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

        private static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings().ConfigureForHalite());
        }
    }
}