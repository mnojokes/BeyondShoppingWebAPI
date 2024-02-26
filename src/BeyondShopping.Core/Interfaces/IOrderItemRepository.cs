using BeyondShopping.Core.Models;
using System.Data;

namespace BeyondShopping.Core.Interfaces;

public interface IOrderItemRepository
{
    Task Create(OrderItemModel orderItem, IDbTransaction? transaction = null);
    Task Delete(int orderId, IDbTransaction? transaction = null);
}
