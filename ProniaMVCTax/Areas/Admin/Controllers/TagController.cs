using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProniaMVCTax.Areas.Admin.ViewModels;
using ProniaMVCTax.Models;

namespace ProniaMVCTax.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Moderator")]
public class TagController : Controller
{
    private readonly AppDbContext _context;

    public TagController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        List<Tag> tags = _context.Tags.ToList();
        return View(tags);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(TagCreateVM tagCreateVM)
    {
        if (!ModelState.IsValid)
        {
            return View(tagCreateVM);
        }

        Tag tag = new Tag()
        {
            Name = tagCreateVM.Name,
        };
        _context.Tags.Add(tag);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Update(int id)
    {
        Tag? tag = _context.Tags.Find(id);
        if (tag == null) return NotFound();
        TagUpdateVM tagUpdateVM = new TagUpdateVM()
        {
            Name = tag.Name,
        };
        return View(tagUpdateVM);
    }

    [HttpPost]
    public IActionResult Update(TagUpdateVM tagUpdateVM)
    {
        if (!ModelState.IsValid)
        {
            return View(tagUpdateVM);
        }

        Tag? baseTag = _context.Tags.Find(tagUpdateVM.Id);
        if (baseTag == null) return NotFound();

        baseTag.Name = tagUpdateVM.Name;

        _context.Tags.Update(baseTag);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        Tag? tag = _context.Tags.Find(id);
        if (tag == null) return NotFound();
        _context.Tags.Remove(tag);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}

