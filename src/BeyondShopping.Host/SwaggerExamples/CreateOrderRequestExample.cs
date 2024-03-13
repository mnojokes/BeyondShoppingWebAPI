using BeyondShopping.Contracts.Objects;
using BeyondShopping.Contracts.Requests;
using Swashbuckle.AspNetCore.Filters;

#pragma warning disable 1591

namespace BeyondShopping.Host.SwaggerExamples;

public class CreateOrderRequestExample : IExamplesProvider<CreateOrderRequest>
{
    public CreateOrderRequest GetExamples()
    {
        return new CreateOrderRequest()
        {
            UserId = 123,
            Items = new List<OrderItem>()
            {
                new OrderItem(3, 12),
                new OrderItem(12, 48),
                new OrderItem(1, 4)
            }
        };
    }
}
