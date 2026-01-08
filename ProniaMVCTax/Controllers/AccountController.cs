using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProniaMVCTax.Abstractions;
using ProniaMVCTax.Models;
using ProniaMVCTax.ViewModels;
using System.Threading.Tasks;

namespace ProniaMVCTax.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _emailService = emailService;
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

        result = await _userManager.AddToRoleAsync(appUser, "Member");

        await SendConfirmationEmailAsync(appUser);
        
        TempData["SuccessMessage"] = "Registration successful! Please confirm your account.";

        return RedirectToAction("Login");
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

        if(!appUser.EmailConfirmed)
        {
            TempData["ErrorMessage"] = "Please confirm your email address";
            await SendConfirmationEmailAsync(appUser);
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

    public async Task<IActionResult> CreateAdminAndModerator()
    {
        var adminUserVM = _configuration.GetSection("AdminUser").Get<UserVM>();

        if (adminUserVM is {})
        {
            AppUser admin = new AppUser
            {
                FirstName = adminUserVM.FirstName,
                LastName = adminUserVM.LastName,
                UserName = adminUserVM.UserName,
                Email = adminUserVM.Email,

            };
            var result = await _userManager.CreateAsync(admin, adminUserVM.Password);
            result = await _userManager.AddToRoleAsync(admin, "Admin");

        }

        var moderatorUserVM = _configuration.GetSection("ModeratorUser").Get<UserVM>();

        if (moderatorUserVM is { })
        {
            AppUser moderator = new AppUser
            {
                FirstName = moderatorUserVM.FirstName,
                LastName = moderatorUserVM.LastName,
                UserName = moderatorUserVM.UserName,
                Email = moderatorUserVM.Email,

            };
            var result = await _userManager.CreateAsync(moderator, moderatorUserVM.Password);
            result = await _userManager.AddToRoleAsync(moderator, "Moderator");

        }

        return RedirectToAction(nameof(Index), "Home");
    }

    public async Task<IActionResult> CreateRoles()
    {
        await _roleManager.CreateAsync(new IdentityRole("Admin"));
        await _roleManager.CreateAsync(new IdentityRole("Moderator"));
        await _roleManager.CreateAsync(new IdentityRole("Member"));
        return RedirectToAction(nameof(Index), "Home");
    }

    public async Task<IActionResult> ConfirmEmail(string token,string userId)
    {
        var user =  await _userManager.FindByIdAsync(userId);
        if(user is null)
            return BadRequest();

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if(!result.Succeeded)
            return BadRequest();

        await _signInManager.SignInAsync(user, false);

        return RedirectToAction(nameof(Index), "Home");
    }

    private async Task SendConfirmationEmailAsync(AppUser appUser)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = appUser.Id, token = token }, Request.Scheme) ?? string.Empty;

        var subject = "Please confirm your email";

        var body = $@"
<!doctype html>
<html lang=""az"">
<head>
  <meta charset=""utf-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
  <meta name=""x-apple-disable-message-reformatting"" />
  <title>Pronia – Email Təsdiqi</title>
</head>

<body style=""margin:0;padding:0;background:#f3f4f6;font-family:Arial,Helvetica,sans-serif;color:#111827;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">
    <tr>
      <td align=""center"" style=""padding:32px 16px;"">

        <table width=""600"" style=""max-width:600px;width:100%;background:#ffffff;border-radius:14px;border:1px solid #e5e7eb;overflow:hidden;"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">

          <!-- Header -->
          <tr>
            <td style=""background:#16a34a;padding:22px;color:#ffffff;"">
              <div style=""font-size:18px;font-weight:700;"">Pronia</div>
              <div style=""font-size:13px;opacity:0.9;margin-top:4px;"">
                Hesab Təsdiqləmə
              </div>
            </td>
          </tr>

          <!-- Content -->
          <tr>
            <td style=""padding:24px;"">
              <h1 style=""margin:0 0 12px 0;font-size:20px;color:#111827;"">
                Email ünvanınızı təsdiqləyin
              </h1>

              <p style=""margin:0 0 16px 0;font-size:14px;line-height:1.6;color:#374151;"">
                Pronia hesabınızı aktivləşdirmək üçün aşağıdakı düyməyə klikləyin.
              </p>

              <!-- Button -->
              <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" style=""margin:20px 0;"">
                <tr>
                  <td align=""center"" bgcolor=""#16a34a"" style=""border-radius:10px;"">
                    <a href=""{confirmationLink}""
                       target=""_blank""
                       style=""display:inline-block;padding:12px 22px;font-size:14px;font-weight:700;color:#ffffff;text-decoration:none;border-radius:10px;"">
                      Emaili təsdiqlə
                    </a>
                  </td>
                </tr>
              </table>

              <p style=""margin:0 0 8px 0;font-size:12px;color:#6b7280;"">
                Düymə işləmirsə, bu linki brauzerinizə yapışdırın:
              </p>

              <p style=""margin:0 0 18px 0;font-size:12px;word-break:break-all;"">
                <a href=""{confirmationLink}"" target=""_blank"" style=""color:#16a34a;text-decoration:underline;"">
                  {confirmationLink}
                </a>
              </p>

              <div style=""height:1px;background:#e5e7eb;margin:18px 0;""></div>

              <p style=""margin:0;font-size:12px;color:#6b7280;line-height:1.6;"">
                Əgər bu sorğunu siz etməmisinizsə, bu emaili nəzərə almayın.
              </p>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style=""background:#f9fafb;padding:16px 24px;border-top:1px solid #e5e7eb;"">
              <p style=""margin:0;font-size:11px;color:#9ca3af;"">
                © {DateTime.UtcNow:yyyy} Pronia. Bütün hüquqlar qorunur.
              </p>
            </td>
          </tr>

        </table>

        <p style=""margin-top:12px;font-size:11px;color:#9ca3af;text-align:center;"">
          Bu avtomatik göndərilən mesajdır, cavab yazmayın.
        </p>

      </td>
    </tr>
  </table>
</body>
</html>";


        await _emailService.SendEmailAsync(appUser.Email!, subject, body);

    }
}
