using System.Security.Claims;
using System.Threading.Tasks;
using BanHangDienTu.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BanHangDienTu.Controllers;

[Authorize]
public class OrderHistoryController : Controller
{
    private readonly IOrderService _orderService;

    public OrderHistoryController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? status = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var orders = await _orderService.GetOrderHistoryAsync(userId, status);
        ViewBag.CurrentStatus = string.IsNullOrWhiteSpace(status) ? "Tất cả" : status;

        return View(orders);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var result = await _orderService.CancelOrderAsync(id, userId);

        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Không thể hủy đơn hàng.";
        }
        else
        {
            TempData["SuccessMessage"] = $"Đơn hàng #{id} đã được hủy thành công.";
        }

        return RedirectToAction(nameof(Index));
    }
}