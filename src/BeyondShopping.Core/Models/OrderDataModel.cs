namespace BeyondShopping.Core.Models;

public record OrderDataModel
{
    public int Id { get; init; } = default;
    public int UserId { get; init; } = default;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; } = default;

    public OrderDataModel() { }
    public OrderDataModel(int id, int userId, string status, DateTime createdAt)
        => (Id, UserId, Status, CreatedAt) = (id, userId, status, createdAt);
}
