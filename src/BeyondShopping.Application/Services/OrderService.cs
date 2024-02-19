using BeyondShopping.Contracts.Requests;
using BeyondShopping.Contracts.Responses;

namespace BeyondShopping.Application.Services;

public class OrderService
{
    public Task<OrderResponse> CreateOrder(CreateOrderRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<OrderResponse> CompleteOrder(int id)
    {
        throw new NotImplementedException();
    }

    public Task<OrderResponseList> GetUserOrders(int userId)
    {
        throw new NotImplementedException();
    }
}
