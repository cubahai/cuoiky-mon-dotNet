using Microsoft.AspNetCore.Mvc;

namespace BanHangDienTu.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
