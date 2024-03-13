using BeyondShopping.Contracts.Objects;

namespace BeyondShopping.Core.Interfaces;

public interface IUserRepository
{
    Task<UserData?> Get(int id);
}
