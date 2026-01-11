
using Microsoft.EntityFrameworkCore;
using ProniaMVCTax.Abstractions;
using ProniaMVCTax.Models;
using System.Security.Claims;

namespace ProniaMVCTax.Services;

public class BasketService:IBasketService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public BasketService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<BasketItem>> GetBasketItemsAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var isExistUser = await _context.Users.AnyAsync(u => u.Id == userId);

        if (!isExistUser)
            return [];

        var basketItems = await _context.BasketItems
            .Where(b => b.AppUserId == userId)
            .Include(b => b.Product)
                .ThenInclude(p => p.ProductImages)
            .ToListAsync();

        return basketItems;
    }

    public async Task<decimal> GetBasketTotalAsync()
    {
        var basketItems = await GetBasketItemsAsync();
        decimal total = 0;
        foreach (var item in basketItems)
        {
            total += item.Product.Price * item.Count;
        }
        return total;
    }
}
