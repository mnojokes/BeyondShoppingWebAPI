using BeyondShopping.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;

#pragma warning disable 1591

namespace BeyondShopping.Host.SwaggerExamples;

public class ErrorResponseExample : IExamplesProvider<ErrorResponse>
{
    public ErrorResponse GetExamples()
    {
        return new ErrorResponse("Something truly terrible just happened.");
    }
}
