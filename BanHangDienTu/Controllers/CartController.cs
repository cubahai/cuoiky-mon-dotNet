using Microsoft.AspNetCore.Mvc;
using BanHangDienTu.Models;

namespace BanHangDienTu.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            // Khởi tạo danh sách rỗng, sẵn sàng kết nối với Session / Database
            var cartItems = new List<CartItemViewModel>();
            return View(cartItems);
        }
    }
}
