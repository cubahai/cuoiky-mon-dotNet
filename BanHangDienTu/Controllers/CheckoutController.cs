using System.Security.Claims;
using BanHangDienTu.Models.Constants;
using BanHangDienTu.Services.Interfaces;
using BanHangDienTu.ViewModels.Checkout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BanHangDienTu.Controllers;

[Authorize(Roles = AppRoles.Customer)]
[AutoValidateAntiforgeryToken]
public class CheckoutController(IOrderService orders) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Challenge();
        var model = await orders.GetCheckoutAsync(userId);
        if (!model.Cart.CanCheckout)
        {
            TempData["ErrorMessage"] = "Vui lòng kiểm tra sản phẩm và số lượng trong giỏ trước khi đặt hàng.";
            return RedirectToAction("Index", "Cart");
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(CheckoutViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Challenge();
        if (ModelState.IsValid)
        {
            var result = await orders.PlaceAsync(userId, model);
            if (result.OrderId is int id) return RedirectToAction(nameof(Success), new { id });
            ModelState.AddModelError(string.Empty, result.Error ?? "Không thể tạo đơn hàng.");
        }
        // Always render fresh prices and issue a new confirmation after a failed attempt.
        var fresh = await orders.GetCheckoutAsync(userId);
        model.Cart = fresh.Cart;
        model.CheckoutToken = fresh.CheckoutToken;
        ModelState.Remove(nameof(model.CheckoutToken));
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Success(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Challenge();
        var model = await orders.GetDetailAsync(userId, id);
        return model is null ? NotFound() : View(model);
    }
}
