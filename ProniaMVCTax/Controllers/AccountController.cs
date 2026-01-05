using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProniaMVCTax.Models;
using ProniaMVCTax.ViewModels;
using System.Threading.Tasks;

namespace ProniaMVCTax.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM registerVM)
    {
        if (!ModelState.IsValid)
        {
            return View(registerVM);
        }

        AppUser appUser = new AppUser
        {
            FirstName = registerVM.FirstName,
            LastName = registerVM.LastName,
            Email = registerVM.EmailAddress,
            UserName = registerVM.UserName,
        };

        var result = await _userManager.CreateAsync(appUser, registerVM.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
                return View(registerVM);
            }
        }
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM loginVM)
    {
        if (!ModelState.IsValid)
        {
            return View(loginVM);
        }

        var appUser = await _userManager.FindByEmailAsync(loginVM.EmailAddress);
        if (appUser == null)
        {
            ModelState.AddModelError("", "Invalid Email or Password");
            return View(loginVM);
        }

        var isCorrectPassword = await _userManager.CheckPasswordAsync(appUser, loginVM.Password);
        if (!isCorrectPassword)
        {
            ModelState.AddModelError("", "Invalid Email or Password");
            return View(loginVM);
        }

        await _signInManager.SignInAsync(appUser, loginVM.RememberMe);

        return RedirectToAction(nameof(Index), "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Index), "Home");
    }
}
