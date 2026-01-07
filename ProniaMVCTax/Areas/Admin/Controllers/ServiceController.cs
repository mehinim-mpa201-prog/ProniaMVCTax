using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProniaMVCTax.Areas.Admin.ViewModels;
using ProniaMVCTax.Models;
using System.IO;

namespace ProniaMVCTax.Areas.Admin.Controllers;

[Area("Admin")]

[Authorize(Roles = "Admin,Moderator")]
public class ServiceController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ServiceController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
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
        if(!ModelState.IsValid) return View(createVM);

        Service service = new Service
        {
            Title = createVM.Title,
            ShortDescription = createVM.ShortDescription,
        };

        string path = Path.Combine(_env.WebRootPath, "admin", "assets", "images", "uploads");
        string iconImageName = Guid.NewGuid() + createVM.IconImage.FileName;
        using FileStream stream = new(Path.Combine(path, iconImageName), FileMode.Create);
        createVM.IconImage.CopyTo(stream);
        service.IconImgPath = iconImageName;


        _context.Services.Add(service);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Update(int id)
    {
        Service? service = _context.Services.Find(id);
        if (service == null) return NotFound();

        ServiceUpdateVM serviceUpdateVM = new ServiceUpdateVM
        {
            Id = service.Id,
            Title = service.Title,
            ShortDescription = service.ShortDescription
        };

        return View(serviceUpdateVM);
    }

    [HttpPost]
    public IActionResult Update(ServiceUpdateVM serviceUpdateVM)
    {
        if (!ModelState.IsValid) return View(serviceUpdateVM);
        Service? baseService = _context.Services.Find(serviceUpdateVM.Id);
        if (baseService is null) return NotFound();

        baseService.Title = serviceUpdateVM.Title;
        baseService.ShortDescription = serviceUpdateVM.ShortDescription;


        string path = Path.Combine(_env.WebRootPath, "admin", "assets", "images", "uploads");
        if (serviceUpdateVM.IconImage != null)
        {
            if (System.IO.File.Exists(Path.Combine(path, baseService.IconImgPath)))
                System.IO.File.Delete(Path.Combine(path, baseService.IconImgPath));


            string iconImgPath = Guid.NewGuid() + serviceUpdateVM.IconImage.FileName;
            using FileStream stream = new(Path.Combine(path, iconImgPath), FileMode.Create);
            serviceUpdateVM.IconImage.CopyTo(stream);
            baseService.IconImgPath = iconImgPath;

        }

        _context.Services.Update(baseService);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        Service? service = _context.Services.Find(id);
        if (service == null) return NotFound();
        _context.Services.Remove(service);
        _context.SaveChanges();


        string path = Path.Combine(_env.WebRootPath, "admin", "assets", "images", "uploads");
        if (System.IO.File.Exists(Path.Combine(path, service.IconImgPath)))
            System.IO.File.Delete(Path.Combine(path, service.IconImgPath));


        return RedirectToAction("Index");
    }

}
