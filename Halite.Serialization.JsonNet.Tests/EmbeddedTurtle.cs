namespace Halite.Serialization.JsonNet.Tests;

public class EmbeddedTurtle : HalEmbedded
{
    [HalRelation("down")]
    public TurtleResource? Down { get; set; }
}