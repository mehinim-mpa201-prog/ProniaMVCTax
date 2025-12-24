using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVCTax.Areas.Admin.ViewModels;
using ProniaMVCTax.Models;

namespace ProniaMVCTax.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        List<Category> categories = _context.Categories.ToList();
        return View(categories);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(CategoryCreateVM categoryCreateVM)
    {
        if (!ModelState.IsValid)
        {
            return View(categoryCreateVM);
        }
        Category category = new();
        category.Name = categoryCreateVM.Name;

        _context.Categories.Add(category);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public IActionResult Update(int id)
    {
        Category? category = _context.Categories.Find(id);
        if (category == null) return NotFound();
        return View(category);
    }

    [HttpPost]
    public IActionResult Update(Category category)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }


        Category? baseCategory = _context.Categories.Find(category.Id);
        if (baseCategory == null) return NotFound();

        baseCategory.Name = category.Name;

        _context.Categories.Update(baseCategory);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));

    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        Category? category = _context.Categories.Find(id);
        if (category == null) return NotFound();
        _context.Categories.Remove(category);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
