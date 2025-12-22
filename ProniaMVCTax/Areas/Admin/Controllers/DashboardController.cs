using Microsoft.AspNetCore.Mvc;
using ProniaMVCTax.Areas.Admin.ViewModels;
using ProniaMVCTax.Models;

namespace ProniaMVCTax.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        List<Service> services = _context.Services.ToList();
        return View(services);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(ServiceCreateVM createVM)
    {
        Service service = new Service
        {
            Title = createVM.Title,
            ShortDescription = createVM.ShortDescription,
            IconImgPath = createVM.IconImgPath
        };
        _context.Services.Add(service);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        Service? service = _context.Services.Find(id);
        if (service == null) return NotFound();
        _context.Services.Remove(service);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

   
}
