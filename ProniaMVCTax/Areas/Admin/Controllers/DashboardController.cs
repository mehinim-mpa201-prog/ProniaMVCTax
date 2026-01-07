using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProniaMVCTax.Areas.Admin.ViewModels;
using ProniaMVCTax.Models;

namespace ProniaMVCTax.Areas.Admin.Controllers;

[Area("Admin")]

[Authorize(Roles = "Admin,Moderator")]
public class DashboardController : Controller
{
   public IActionResult Index()
   {
       return View();
    }

}
