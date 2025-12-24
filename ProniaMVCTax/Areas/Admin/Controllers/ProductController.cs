using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProniaMVCTax.Areas.Admin.ViewModels;
using ProniaMVCTax.Models;

namespace ProniaMVCTax.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly AppDbContext _context;

    public ProductController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        List<Product> products = _context.Products.Include(p => p.Category).ToList();
        return View(products);
    }

    [HttpGet]
    public IActionResult Create()
    {

        List<Category> categories = _context.Categories.ToList();
        ViewBag.Categories = categories;

        return View();
    }

    [HttpPost]
    public IActionResult Create(ProductCreateVM productCreateVM)
    {
        if (!ModelState.IsValid)
        {
            List<Category> categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            return View(productCreateVM);
        }
        Product product = new();
        product.Name = productCreateVM.Name;
        product.Description = productCreateVM.Description;
        product.CategoryId = productCreateVM.CategoryId;
        product.Price = productCreateVM.Price;
        product.SKU = productCreateVM.SKU;
        product.MainImagePath = productCreateVM.MainImagePath;
        product.HoverImagePath = productCreateVM.HoverImagePath;

        _context.Products.Add(product);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public IActionResult Update(int id)
    {
        Product? product = _context.Products.Find(id);
        if (product == null) return NotFound();
        List<Category> categories = _context.Categories.ToList();
        ViewBag.Categories = categories;
        return View(product);
    }

    [HttpPost]
    public IActionResult Update(Product product)
    {
        if (!ModelState.IsValid)
        {
            List<Category> categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            return View(product);
        }

        var isCategoryExists = _context.Categories.Any(c => c.Id == product.CategoryId);
        if(!isCategoryExists)
        {
            ModelState.AddModelError("CategoryId", "Seçilmiş Category Databasedə tapılmadı.");
            List<Category> categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            return View(product);
        }

        Product? baseProduct = _context.Products.Find(product.Id);
        if (baseProduct == null) return NotFound();
        baseProduct.Name = product.Name;
        baseProduct.Description = product.Description;
        baseProduct.CategoryId = product.CategoryId;
        baseProduct.Price = product.Price;
        baseProduct.SKU = product.SKU;
        baseProduct.MainImagePath = product.MainImagePath;
        baseProduct.HoverImagePath = product.HoverImagePath;

        _context.Products.Update(baseProduct);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));

    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        Product? product = _context.Products.Find(id);
        if (product == null) return NotFound();
        _context.Products.Remove(product);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
