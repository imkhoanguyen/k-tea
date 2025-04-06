using Tea.Domain.Entities;

namespace Tea.Application.Interfaces
{
    public interface ICartService
    {
        Task<bool> DeleteCartAsync(string key);
        Task<ShoppingCart?> GetCartAsync(string key);
        Task<ShoppingCart?> SetCartAsync(ShoppingCart cart);
    }
}