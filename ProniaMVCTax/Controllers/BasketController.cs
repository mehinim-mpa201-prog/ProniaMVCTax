using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVCTax.Abstractions;
using ProniaMVCTax.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProniaMVCTax.Controllers;

[Authorize]
public class BasketController : Controller
{
    private readonly AppDbContext _context;
    private readonly IBasketService _basketService;

    public BasketController(AppDbContext context, IBasketService basketService)
    {
        _context = context;
        _basketService = basketService;
    }

    public async Task<IActionResult> Index()
    {
        var basketItems = await _basketService.GetBasketItemsAsync();
        return View(basketItems);
    }

    public async Task<IActionResult> AddToBasket(int productId)
    {
        var isExistProduct = await _context.Products.AnyAsync(b => b.Id == productId);
        if (!isExistProduct)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isExistUser = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!isExistUser)
            return NotFound();

        var existBasketItem = await _context.BasketItems
            .FirstOrDefaultAsync(b => b.ProductId == productId && b.AppUserId == userId);

        if (existBasketItem is not null)
        {
            existBasketItem.Count += 1;
            _context.BasketItems.Update(existBasketItem);
        }
        else
        {
            BasketItem basketItem = new BasketItem
            {
                ProductId = productId,
                AppUserId = userId,
                Count = 1
            };

            await _context.BasketItems.AddAsync(basketItem);
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Product Successfully added";

        var returnUrl = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Shop");
    }

    public async Task<IActionResult> RemoveFromBasket(int productId)
    {

        var isExistProduct = await _context.Products.AnyAsync(b => b.Id == productId);
        if (!isExistProduct)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isExistUser = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!isExistUser)
            return NotFound();

        var existBasketItem = await _context.BasketItems.FirstOrDefaultAsync(b => b.ProductId == productId && b.AppUserId == userId);
        if (existBasketItem == null)
        {
            return NotFound();
        }
        _context.BasketItems.Remove(existBasketItem);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Product Successfully removed";

        var returnUrl = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Shop");
    }


    public async Task<IActionResult> DecreaseBasketItemCount(int productId)
    {
        var isExistProduct = await _context.Products.AnyAsync(b => b.Id == productId);
        if (!isExistProduct)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isExistUser = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!isExistUser)
            return NotFound();

        var existBasketItem = await _context.BasketItems.FirstOrDefaultAsync(b => b.ProductId == productId && b.AppUserId == userId);
        if (existBasketItem == null)
        {
            return NotFound();
        }


        if (existBasketItem.Count > 1)
        {
            existBasketItem.Count -= 1;
            _context.BasketItems.Update(existBasketItem);
        }
        else
        {
            _context.BasketItems.Remove(existBasketItem);
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Basket item count successfully decreased";
        var returnUrl = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Shop");
    }
}