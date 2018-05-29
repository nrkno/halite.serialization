namespace Halite.Tests
{
    internal class DtoLinks : HalLinks
    {
        public DtoLinks(SelfLink self) : base(self)
        {
        }

        public HalLink Link1 { get; set; }

        public ThatLink Link2 { get; set; }
    }
}