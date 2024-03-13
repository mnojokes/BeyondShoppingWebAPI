namespace BeyondShopping.Contracts.Objects;

public record OrderItem
{
    public int Id { get; init; } = default;
    public int Quantity { get; init; } = default;

    public OrderItem() { }
    public OrderItem(int id, int quantity)
        => (Id, Quantity) = (id, quantity);
}
