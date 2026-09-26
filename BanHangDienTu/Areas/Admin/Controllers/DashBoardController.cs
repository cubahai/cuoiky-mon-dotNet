using BanHangDienTu.Data;
using BanHangDienTu.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BanHangDienTu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashBoardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashBoardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var model = new DashBoardViewModel
            {
                TotalProducts = _context.Products.Count(),
                LowStockCount = _context.Products.Count(p => p.StockQuantity < 5),

                // Dữ liệu giả lập, sẽ thay thế khi có bảng Order
                PendingOrders = 12,
                MonthlyRevenue = 84500000
            };

            return View(model);
        }
    }

}

