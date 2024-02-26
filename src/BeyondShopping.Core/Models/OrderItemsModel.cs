namespace BeyondShopping.Core.Models;

public record OrderItemModel
{
    public int OrderId { get; init; } = default;
    public int ItemId { get; init; } = default;
    public int Quantity { get; init; } = default;

    public OrderItemModel() { }
    public OrderItemModel(int orderId, int itemId, int quantity)
        => (OrderId, ItemId, Quantity) = (orderId, itemId, quantity);
}
