using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProniaMVCTax.Areas.Admin.ViewModels;
using ProniaMVCTax.Models;
using System.IO;

namespace ProniaMVCTax.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ProductController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
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
        if (productCreateVM.CategoryId == 0)
        {
            ModelState.AddModelError("CategoryId", "Zəhmət olmasa bir Category seçin.");
            List<Category> categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            return View(productCreateVM);
        }
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
        product.Star = productCreateVM.Star;

        
        string path = Path.Combine(_env.WebRootPath, "admin", "assets", "images", "uploads");

        string mainImageName = Guid.NewGuid() + productCreateVM.MainImage.FileName;
        using FileStream stream = new(Path.Combine(path,mainImageName), FileMode.Create);
        productCreateVM.MainImage.CopyTo(stream);
        product.MainImagePath = mainImageName;

        string hoverImage = Guid.NewGuid() + productCreateVM.HoverImage.FileName;
        using FileStream stream2 = new(Path.Combine(path, hoverImage), FileMode.Create);
        productCreateVM.HoverImage.CopyTo(stream2);
        product.HoverImagePath = hoverImage;

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

        ProductUpdateVM productUpdateVM = new ProductUpdateVM
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            CategoryId = product.CategoryId,
            Price = product.Price,
            SKU = product.SKU,
            Star = product.Star
        };
        return View(productUpdateVM);
    }

    [HttpPost]
    public IActionResult Update(ProductUpdateVM productUpdateVM)
    {
        if (!ModelState.IsValid)
        {
            List<Category> categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            return View(productUpdateVM);
        }

        var isCategoryExists = _context.Categories.Any(c => c.Id == productUpdateVM.CategoryId);
        if (!isCategoryExists)
        {
            ModelState.AddModelError("CategoryId", "Seçilmiş Category Databasedə tapılmadı.");
            List<Category> categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            return View(productUpdateVM);
        }

        Product? baseProduct = _context.Products.Find(productUpdateVM.Id);
        if (baseProduct == null) return NotFound();

        baseProduct.Name = productUpdateVM.Name;
        baseProduct.Description = productUpdateVM.Description;
        baseProduct.CategoryId = productUpdateVM.CategoryId;
        baseProduct.Price = productUpdateVM.Price;
        baseProduct.SKU = productUpdateVM.SKU;
        baseProduct.Star = productUpdateVM.Star;



        string path = Path.Combine(_env.WebRootPath, "admin", "assets", "images", "uploads");
        if (productUpdateVM.MainImage != null)
        {
            if (System.IO.File.Exists(Path.Combine(path, baseProduct.MainImagePath)))
                System.IO.File.Delete(Path.Combine(path, baseProduct.MainImagePath));


            string mainImageName = Guid.NewGuid() + productUpdateVM.MainImage.FileName;
            using FileStream stream = new(Path.Combine(path, mainImageName), FileMode.Create);
            productUpdateVM.MainImage.CopyTo(stream);
            baseProduct.MainImagePath = mainImageName;

        }

        if (productUpdateVM.HoverImage != null)
        {
            if (System.IO.File.Exists(Path.Combine(path, baseProduct.HoverImagePath)))
                System.IO.File.Delete(Path.Combine(path, baseProduct.HoverImagePath));

            string hoverImageName = Guid.NewGuid() + productUpdateVM.HoverImage.FileName;
            using FileStream stream = new(Path.Combine(path, hoverImageName), FileMode.Create);
            productUpdateVM.HoverImage.CopyTo(stream);
            baseProduct.HoverImagePath = hoverImageName;
        }

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


        string path = Path.Combine(_env.WebRootPath, "admin", "assets", "images", "uploads");

        if (System.IO.File.Exists(Path.Combine(path, product.MainImagePath)))
            System.IO.File.Delete(Path.Combine(path, product.MainImagePath));

        if (System.IO.File.Exists(Path.Combine(path, product.HoverImagePath)))
            System.IO.File.Delete(Path.Combine(path, product.HoverImagePath));

        return RedirectToAction(nameof(Index));
    }
}
