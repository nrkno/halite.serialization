namespace Halite.Serialization.Examples;

using System.Collections.Generic;

public class OrdersEmbedded : HalEmbedded
{
    public OrdersEmbedded(IReadOnlyList<OrderLineResource> orderLines)
    {
        OrderLines = orderLines;
    }

    [HalRelation("ea:order")]
    public IReadOnlyList<OrderLineResource> OrderLines { get; }
}