using System.Security.Claims;
using System.Threading.Tasks;
using BanHangDienTu.Models;
using BanHangDienTu.Models.Entities;
using BanHangDienTu.Services.Interfaces;
using BanHangDienTu.ViewModels.Cart;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BanHangDienTu.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartController(
        ICartService cartService,
        IOrderService orderService,
        UserManager<ApplicationUser> userManager)
    {
        _cartService = cartService;
        _orderService = orderService;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var cartItems = _cartService.GetCart();
        return View(cartItems);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(int productId, int quantity = 1, string? returnUrl = null)
    {
        var result = await _cartService.AddToCartAsync(productId, quantity);

        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Không thể thêm sản phẩm vào giỏ hàng.";
            return RedirectToAction("Details", "Product", new { id = productId });
        }

        TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng thành công!";

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateQuantity(int productId, int quantity)
    {
        _cartService.UpdateQuantity(productId, quantity);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveItem(int productId)
    {
        _cartService.RemoveItem(productId);
        TempData["SuccessMessage"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        _cartService.ClearCart();
        TempData["SuccessMessage"] = "Đã xóa toàn bộ giỏ hàng.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BuyNow(int productId, int quantity = 1)
    {
        var result = await _cartService.SetBuyNowItemAsync(productId, quantity);

        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Không thể mua sản phẩm này.";
            return RedirectToAction("Details", "Product", new { id = productId });
        }

        return RedirectToAction(nameof(Checkout), new { buyNow = true });
    }

    [HttpGet]
    public async Task<IActionResult> Checkout(bool buyNow = false)
    {
        List<CartItemViewModel> items;
        decimal grandTotal;

        if (buyNow)
        {
            var buyNowItem = _cartService.GetBuyNowItem();
            if (buyNowItem is null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin sản phẩm mua ngay.";
                return RedirectToAction("Index", "Home");
            }

            items = new List<CartItemViewModel> { buyNowItem };
            grandTotal = buyNowItem.TotalPrice;
        }
        else
        {
            var cartItems = _cartService.GetCart();
            if (cartItems.Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống. Vui lòng chọn sản phẩm trước khi thanh toán.";
                return RedirectToAction(nameof(Index));
            }

            items = cartItems;
            grandTotal = _cartService.GetGrandTotal();
        }

        var model = new CheckoutViewModel
        {
            IsBuyNow = buyNow,
            Items = items,
            GrandTotal = grandTotal
        };

        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    model.CustomerName = user.FullName;
                    model.PhoneNumber = user.PhoneNumber ?? string.Empty;
                    model.ShippingAddress = user.Address ?? string.Empty;
                }
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        List<CartItemViewModel> items;

        if (model.IsBuyNow)
        {
            var buyNowItem = _cartService.GetBuyNowItem();
            if (buyNowItem is null)
            {
                TempData["ErrorMessage"] = "Phiên mua ngay đã hết hạn. Vui lòng thử lại.";
                return RedirectToAction("Index", "Home");
            }

            items = new List<CartItemViewModel> { buyNowItem };
            model.GrandTotal = buyNowItem.TotalPrice;
        }
        else
        {
            var cartItems = _cartService.GetCart();
            if (cartItems.Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction(nameof(Index));
            }

            items = cartItems;
            model.GrandTotal = _cartService.GetGrandTotal();
        }

        model.Items = items;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = User.Identity?.IsAuthenticated == true
            ? User.FindFirstValue(ClaimTypes.NameIdentifier)
            : null;

        var result = await _orderService.CreateOrderAsync(userId, model, items);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Đặt hàng không thành công.");
            return View(model);
        }

        if (model.IsBuyNow)
        {
            // Chỉ xóa phiên mua ngay, giữ nguyên 100% giỏ hàng chính
            _cartService.ClearBuyNowItem();
        }
        else
        {
            _cartService.ClearCart();
        }

        return RedirectToAction(nameof(Success), new { orderId = result.OrderId });
    }

    [HttpGet]
    public async Task<IActionResult> Success(int orderId)
    {
        var userId = User.Identity?.IsAuthenticated == true
            ? User.FindFirstValue(ClaimTypes.NameIdentifier)
            : null;

        var order = await _orderService.GetOrderDetailAsync(orderId, userId);
        if (order == null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(order);
    }
}