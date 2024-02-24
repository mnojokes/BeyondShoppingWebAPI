namespace BeyondShopping.Contracts.Requests;

public record CreateOrderRequest
{
    public int UserId { get; init; } = default;
    public List<int>? Items { get; init; } = null;

    public CreateOrderRequest() { }
    public CreateOrderRequest(int userId, List<int> items)
        => (UserId, Items) = (userId, items);
}
