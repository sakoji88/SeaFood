using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeaFood.Services;
using SeaFood.ViewModels;

namespace SeaFood.Controllers;

public class AccountController(IAuthService authService) : Controller
{
    [AllowAnonymous]
    public IActionResult Register() => View(new RegisterViewModel());

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await authService.RegisterAsync(model);
        if (!result.Ok)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            return View(model);
        }

        TempData["Success"] = "Регистрация выполнена. Теперь войдите в систему.";
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    public IActionResult Login() => View(new LoginViewModel());

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await authService.ValidateLoginAsync(model);
        if (!result.Ok || result.User is null)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            return View(model);
        }

        // Cookie содержит claims: Id, имя и роль пользователя для [Authorize].
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.User.Id.ToString()),
            new(ClaimTypes.Name, result.User.FullName),
            new(ClaimTypes.Email, result.User.Email),
            new(ClaimTypes.Role, result.User.Role?.Name ?? "User")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}
