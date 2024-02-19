namespace BeyondShopping.Contracts.Requests;

public record CreateOrderRequest(int UserId, List<int> Items);
