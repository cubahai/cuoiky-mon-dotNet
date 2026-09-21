using BanHangDienTu.Models.Constants;
using BanHangDienTu.Services.Interfaces;
using BanHangDienTu.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;
using System.Threading.Tasks;

namespace BanHangDienTu.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(
        IAccountService accountService)
    {
        _accountService = accountService;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectAuthenticatedUser();
        }

        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        RegisterViewModel model)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectAuthenticatedUser();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result =
            await _accountService.RegisterCustomerAsync(model);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error);
            }

            return View(model);
        }

        TempData["SuccessMessage"] =
            "Đăng ký thành công. Vui lòng đăng nhập.";

        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectAuthenticatedUser();
        }

        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectAuthenticatedUser();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result =
            await _accountService.LoginAsync(model);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(
                string.Empty,
                result.ErrorMessage
                ?? "Đăng nhập không thành công.");

            return View(model);
        }

        if (result.IsAdmin)
        {
            return RedirectToAction(
                "Index",
                "DashBoard",
                new
                {
                    area = "Admin"
                });
        }

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) &&
            Url.IsLocalUrl(model.ReturnUrl))
        {
            return LocalRedirect(model.ReturnUrl);
        }

        return RedirectToAction(
            "Index",
            "Home");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _accountService.LogoutAsync();

        return RedirectToAction(
            "Index",
            "Home");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectAuthenticatedUser()
    {
        if (User.IsInRole(AppRoles.Admin))
        {
            return RedirectToAction(
                "Index",
                "DashBoard",
                new
                {
                    area = "Admin"
                });
        }

        return RedirectToAction(
            "Index",
            "Home");
    }
}