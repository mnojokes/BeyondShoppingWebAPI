namespace BeyondShopping.Contracts.Requests;

public class CreateOrderRequest
{
    public int UserId { get; set; } = default;
    public List<int>? Items { get; set; } = null;
}
