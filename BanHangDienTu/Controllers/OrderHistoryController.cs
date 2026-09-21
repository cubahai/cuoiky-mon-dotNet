using BanHangDienTu.Models.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BanHangDienTu.Controllers;

[Authorize(Roles = AppRoles.Customer)]
public class OrderHistoryController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}