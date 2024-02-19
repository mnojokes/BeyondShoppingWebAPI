namespace BeyondShopping.Core.Models;

public record OrderDataModel(
    int Id,
    int UserId,
    string Status,
    DateTime CreatedAt);
