using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVCTax.Models;
using ProniaMVCTax.ViewModels;

namespace ProniaMVCTax.Controllers;

[Authorize]
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

    public IActionResult Detail(int id)
    {
        List<Service> services = _context.Services.ToList();

        Product? product = _context.Products.Include(p => p.ProductImages)
                                            .Include(p => p.Category)
                                            .Include(p => p.Brand)
                                            .Include(p => p.ProductTags)
                                                .ThenInclude(pt=> pt.Tag)
                                            .FirstOrDefault(p => p.Id == id);
        if (product == null) return NotFound();

        DetailVM detailVM = new DetailVM
        {
            Product = product,
            Services = services
        };
        return View(detailVM);
    }
}
