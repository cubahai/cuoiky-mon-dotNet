using System.Security.Claims;
using BanHangDienTu.Models.Constants;
using BanHangDienTu.Services.Interfaces;
using BanHangDienTu.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BanHangDienTu.Controllers;

[Authorize(Roles = AppRoles.Customer)]
public class ProfileController : Controller
{
    private readonly IAccountService _accountService;

    public ProfileController(
        IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Challenge();
        }

        var model =
            await _accountService.GetProfileAsync(userId);

        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Challenge();
        }

        var model =
            await _accountService.GetProfileAsync(userId);

        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        ProfileViewModel model)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Challenge();
        }

        var currentProfile =
            await _accountService.GetProfileAsync(userId);

        if (currentProfile is null)
        {
            return NotFound();
        }

        model.Email = currentProfile.Email;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result =
            await _accountService.UpdateProfileAsync(
                userId,
                model);

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
            "Cập nhật thông tin thành công.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Challenge();
        }

        var result =
            await _accountService.ChangePasswordAsync(
                userId,
                model);

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
            "Đổi mật khẩu thành công.";

        return RedirectToAction(nameof(Index));
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirstValue(
            ClaimTypes.NameIdentifier);
    }
}