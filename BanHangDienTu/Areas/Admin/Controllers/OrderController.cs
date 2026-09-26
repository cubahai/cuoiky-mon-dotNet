using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BanHangDienTu.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BanHangDienTu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy danh sách đơn hàng, sắp xếp mới nhất lên đầu
            var orders = await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // Action cập nhật trạng thái đơn hàng (Sẽ gọi từ giao diện)
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}