namespace BeyondShopping.Contracts.Responses;

public record OrderResponse(int Id, string Status, DateTime CreatedAt);
public record OrderResponseList(List<OrderResponse> Orders);
