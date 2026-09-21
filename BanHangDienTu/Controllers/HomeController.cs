using BanHangDienTu.Models;
using BanHangDienTu.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BanHangDienTu.Controllers;

public class HomeController : Controller
{
    private readonly IHomeService _homeService;

    public HomeController(IHomeService homeService)
    {
        _homeService = homeService;
    }

    public async Task<IActionResult> Index(string? searchTerm)
    {
        if (searchTerm?.Length > 100)
        {
            ModelState.AddModelError(
                nameof(searchTerm),
                "Từ khóa tìm kiếm không được vượt quá 100 ký tự.");

            searchTerm = null;
        }

        var model = await _homeService.GetHomeDataAsync(searchTerm);

        return View(model);
    }


    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id
                ?? HttpContext.TraceIdentifier
        });
    }
}