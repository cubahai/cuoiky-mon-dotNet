using System.Security.Claims;
using BanHangDienTu.Models.Constants;
using BanHangDienTu.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BanHangDienTu.Controllers;

[Authorize(Roles = AppRoles.Customer)]
public class OrderHistoryController(IOrderService orders) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId is null ? Challenge() : View(await orders.GetHistoryAsync(userId, page));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Challenge();
        var model = await orders.GetDetailAsync(userId, id);
        return model is null ? NotFound() : View(model);
    }
}
