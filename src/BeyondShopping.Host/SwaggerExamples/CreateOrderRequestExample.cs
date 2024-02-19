﻿using BeyondShopping.Contracts.Requests;
using Swashbuckle.AspNetCore.Filters;

#pragma warning disable 1591

namespace BeyondShopping.Host.SwaggerExamples;

public class CreateOrderRequestExample : IExamplesProvider<CreateOrderRequest>
{
    public CreateOrderRequest GetExamples()
    {
        return new CreateOrderRequest(123,
            new List<int> { 3, 18, 42 });
    }
}