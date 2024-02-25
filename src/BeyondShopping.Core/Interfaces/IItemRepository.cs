namespace BeyondShopping.Core.Interfaces;

public interface IItemRepository
{
    Task<string> Get(int id); // TODO: change to actual object type if inventory will be kept
}
