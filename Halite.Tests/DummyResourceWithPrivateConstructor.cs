namespace Halite.Tests;

using Newtonsoft.Json;

internal class DummyResourceWithPrivateConstructor : HalResource<DummyLinks>
{
    internal DummyResourceWithPrivateConstructor Create(DummyLinks dummyLinks)
    {

        var it = new DummyResourceWithPrivateConstructor();
        it.Links = dummyLinks;
        return it;
    }

    [JsonConstructor]
    private DummyResourceWithPrivateConstructor()
    {

    }
}