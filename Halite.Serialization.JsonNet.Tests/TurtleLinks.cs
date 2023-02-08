namespace Halite.Serialization.JsonNet.Tests;

using JetBrains.Annotations;

public class TurtleLinks : HalLinks
{
    public TurtleLinks([CanBeNull] SelfLink self) : base(self)
    {
    }
}