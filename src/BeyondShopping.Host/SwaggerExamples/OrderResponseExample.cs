using BeyondShopping.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;

#pragma warning disable 1591

namespace BeyondShopping.Host.SwaggerExamples;

public class OrderResponseExample : IExamplesProvider<OrderResponse>
{
    public OrderResponse GetExamples()
    {
        return new OrderResponse(123, "Completed", DateTime.Now);
    }
}
