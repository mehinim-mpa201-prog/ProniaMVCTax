using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProniaMVCTax.Areas.Admin.ViewModels;
using ProniaMVCTax.Models;

namespace ProniaMVCTax.Areas.Admin.Controllers;

[Area("Admin")]

[Authorize(Roles = "Admin,Moderator")]

public class BrandController : Controller
{
    private readonly AppDbContext _context;

    public BrandController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        List<Brand> brands = _context.Brands.ToList();
        return View(brands);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(BrandCreateVM brandCreateVM)
    {
        if(!ModelState.IsValid)
        {
            return View(brandCreateVM);
        }

        Brand brand = new Brand()
        {
            Name = brandCreateVM.Name,
        };
        _context.Brands.Add(brand);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Update(int id)
    {
        Brand? brand = _context.Brands.Find(id);
        if(brand == null) return NotFound();
        BrandUpdateVM brandUpdateVM = new BrandUpdateVM()
        {
            Name = brand.Name,
        };
        return View(brandUpdateVM);
    }

    [HttpPost]
    public IActionResult Update(BrandUpdateVM brandUpdateVM)
    {
        if (!ModelState.IsValid)
        {
            return View(brandUpdateVM);
        }

        Brand? baseBrand = _context.Brands.Find(brandUpdateVM.Id);
        if (baseBrand == null) return NotFound();

        baseBrand.Name = brandUpdateVM.Name;

        _context.Brands.Update(baseBrand);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        Brand? brand = _context.Brands.Find(id);
        if (brand == null) return NotFound();
        _context.Brands.Remove(brand);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
