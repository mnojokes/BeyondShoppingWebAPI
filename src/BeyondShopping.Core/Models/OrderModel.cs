namespace BeyondShopping.Core.Models;

public record OrderModel(
    int Id,
    int UserId,
    string Status,
    DateTime CreatedAt);
