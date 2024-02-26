namespace BeyondShopping.Contracts.Responses;

public record OrderResponse
{
    public int Id { get; init; } = default;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; } = default;

    public OrderResponse() { }
    public OrderResponse(int id, string status, DateTime createdAt)
        => (Id, Status, CreatedAt) = (id, status, createdAt);
}

public record OrderResponseList
{
    public List<OrderResponse> Orders { get; init; } = new();

    public OrderResponseList() { }
    public OrderResponseList(List<OrderResponse> orders)
        => Orders = orders;
}
