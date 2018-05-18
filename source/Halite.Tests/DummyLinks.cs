using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Halite.Tests
{
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

    public class MyAttribute : Attribute
    {
        //...
    }

    internal class DummyLinksWithNotNullThis : HalLinks
    {
        public DummyLinksWithNotNullThis(SelfLink self, [NotNull] [My] ThisLink @this, ThatLink that, IReadOnlyList<HalLink> those) : base(self)
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

}