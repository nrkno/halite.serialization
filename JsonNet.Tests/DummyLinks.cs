namespace Halite.Serialization.JsonNet.Tests;

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

internal class DummyLinks : HalLinks
{
    public DummyLinks(SelfLink self, ThisLink @this, ThatLink that, IReadOnlyList<HalLink> those) : base(self)
    {
        This = @this;
        That = that;
        Those = those;
    }

    [HalRelation("this")]
    public ThisLink This { get; }

    [HalRelation("that")]
    public ThatLink That { get; }

    [HalRelation("those")]
    public IReadOnlyList<HalLink> Those { get; }
}

internal class DummyLinksWithNotNullThis : HalLinks
{
    public DummyLinksWithNotNullThis(SelfLink self, [NotNull] ThisLink @this, ThatLink that, IReadOnlyList<HalLink> those) : base(self)
    {
        This = @this;
        That = that;
        Those = those;
    }

    [HalRelation("this")]
    public ThisLink This { get; }

    [HalRelation("that")]
    public ThatLink That { get; }

    [HalRelation("those")]
    public IReadOnlyList<HalLink> Those { get; }
}

internal class DummyLinksWithNullValueHandling : HalLinks
{
    public DummyLinksWithNullValueHandling(SelfLink self, [NotNull] ThisLink @this, ThatLink? that, IReadOnlyList<HalLink> those) : base(self)
    {
        This = @this;
        That = that;
        Those = those;
    }

    [HalRelation("this")]
    public ThisLink This { get; }

    [HalRelation("that")]
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    [CanBeNull]
    public ThatLink? That { get; }

    [HalRelation("those")]
    public IReadOnlyList<HalLink> Those { get; }
}