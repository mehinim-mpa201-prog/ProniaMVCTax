using Microsoft.AspNetCore.Mvc;
using ProniaMVCTax.Models;
using ProniaMVCTax.ViewModels;

namespace ProniaMVCTax.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context = new AppDbContext();
    public IActionResult Index()
    {
        List<Service> services = _context.Services.ToList();
        List<SliderItem> sliderItems = _context.SliderItems.ToList();
        //ViewData["SliderItems"] = sliderItems;

        HomeVM homeVM = new HomeVM
        {
            SliderItems = sliderItems,
            Services = services
        };
        return View(homeVM);
    }
}

