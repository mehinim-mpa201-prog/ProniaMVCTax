using Microsoft.AspNetCore.Mvc;
using ProniaMVCTax.Models;

namespace ProniaMVCTax.Controllers;

public class ShopController : Controller
{
    private readonly AppDbContext _context;

    public ShopController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        List<Product> products = _context.Products.ToList();
        return View(products);
    }
}
