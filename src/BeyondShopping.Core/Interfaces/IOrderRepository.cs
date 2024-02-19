using BeyondShopping.Core.Models;

namespace BeyondShopping.Core.Interfaces;

public interface IOrderRepository
{
    public Task<OrderDataModel> Create(OrderDataModel order);
    public Task<OrderDataModel> UpdateStatus(OrderStatusModel status);
    public Task<IEnumerable<OrderDataModel>> Get(int userId);
    public Task CleanupOlderThan(DateTime time);
}
