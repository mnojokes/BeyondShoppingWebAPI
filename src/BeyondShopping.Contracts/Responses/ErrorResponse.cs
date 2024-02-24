namespace BeyondShopping.Contracts.Responses;

public record ErrorResponse
{
    public string Message { get; init; } = string.Empty;

    public ErrorResponse() { }
    public ErrorResponse(string message)
        => Message = message;
}
