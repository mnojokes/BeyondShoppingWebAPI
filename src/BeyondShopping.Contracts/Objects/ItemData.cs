namespace BeyondShopping.Contracts.Objects;

public record ItemData
{
    public int Id { get; init; } = default;
    public int Quantity { get; init; } = default;

    public ItemData() { }
    public ItemData(int id, int quantity)
        => (Id, Quantity) = (id, quantity);
}
