using ProniaMVCTax.Models;

namespace ProniaMVCTax.Abstractions;

public interface IBasketService
{
    Task<List<BasketItem>> GetBasketItemsAsync();
    Task<decimal> GetBasketTotalAsync();
}
