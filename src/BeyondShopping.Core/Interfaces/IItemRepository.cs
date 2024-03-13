using BeyondShopping.Contracts.Objects;

namespace BeyondShopping.Core.Interfaces;

public interface IItemRepository
{
    Task<OrderItem?> Get(int id);
}
