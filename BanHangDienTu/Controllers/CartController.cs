using BanHangDienTu.Models;
using BanHangDienTu.Models.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BanHangDienTu.Controllers;

[Authorize(Roles = AppRoles.Customer)]
public class CartController : Controller
{
    public IActionResult Index()
    {
        var cartItems =
            new List<CartItemViewModel>();

        return View(cartItems);
    }
}