namespace BeyondShopping.Contracts.Responses;

public class OrderResponse
{
    public int Id { get; set; } = default;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = default;
}

public class OrderResponseList
{
    public List<OrderResponse>? Orders { get; set; } = null;
}
