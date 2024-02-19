using BeyondShopping.Core.Interfaces;
using BeyondShopping.Core.Models;

namespace BeyondShopping.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    public Task CleanupOlderThan(DateTime time)
    {
        throw new NotImplementedException();
    }

    public Task<OrderDataModel> Create(OrderDataModel order)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<OrderDataModel>> Get(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<OrderDataModel> UpdateStatus(OrderStatusModel status)
    {
        throw new NotImplementedException();
    }
}
