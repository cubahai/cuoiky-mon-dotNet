using System.Security.Claims;
using BanHangDienTu.Models.Constants;
using BanHangDienTu.Services.Interfaces;
using BanHangDienTu.ViewModels.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BanHangDienTu.Controllers;

[Authorize(Roles = AppRoles.Customer)]
[AutoValidateAntiforgeryToken]
public class CartController(ICartService cart) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId is null ? Challenge() : View(await cart.GetAsync(userId));
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddCartItemViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Challenge();
        var error = ModelState.IsValid ? await cart.AddAsync(userId, model.ProductId, model.Quantity) : "Sản phẩm hoặc số lượng không hợp lệ (1–999).";
        TempData[error is null ? "SuccessMessage" : "ErrorMessage"] = error ?? "Đã thêm sản phẩm vào giỏ hàng.";
        if (error is null && model.BuyNow) return RedirectToAction("Index", "Checkout");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateCartItemViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Challenge();
        var error = ModelState.IsValid ? await cart.UpdateAsync(userId, model.ProductId, model.Quantity) : "Số lượng phải từ 1 đến 999.";
        TempData[error is null ? "SuccessMessage" : "ErrorMessage"] = error ?? "Đã cập nhật giỏ hàng.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Challenge();
        await cart.RemoveAsync(userId, productId);
        TempData["SuccessMessage"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
        return RedirectToAction(nameof(Index));
    }
}
