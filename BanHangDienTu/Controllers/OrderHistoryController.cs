using Microsoft.AspNetCore.Mvc;
using BanHangDienTu.Models;

namespace BanHangDienTu.Controllers
{
    public class OrderHistoryController : Controller
    {
        public IActionResult Index()
        {
            // Khởi tạo danh sách đơn rỗng, sẵn sàng kết nối DbContext khi có dữ liệu thật
            var orders = new List<OrderViewModel>();
            return View(orders);
        }
    }
}
