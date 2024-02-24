namespace BeyondShopping.Core.Models;

public record OrderStatusModel
{
    public int Id { get; init; } = default;
    public string Status { get; init; } = string.Empty;

    public OrderStatusModel() { }
    public OrderStatusModel(int id, string status)
        => (Id, Status) = (id, status);
}
