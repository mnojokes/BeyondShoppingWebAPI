namespace BeyondShopping.Contracts.Objects;

public record UserData
{
    public int Id { get; init; } = default;
    public string Name { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;

    public UserData() { }
    public UserData(int id, string name, string username, string email)
        => (Id, Name, Username, Email) = (id, name, username, email);
}
