using System.ComponentModel.DataAnnotations;

namespace BanHangDienTu.Models.Enums;

public enum OrderStatus
{
    [Display(Name = "Chờ xác nhận")] Pending = 0,
    [Display(Name = "Đã xác nhận")] Confirmed = 1,
    [Display(Name = "Đang giao hàng")] Shipping = 2,
    [Display(Name = "Đã giao hàng")] Delivered = 3,
    [Display(Name = "Đã hủy")] Cancelled = 4
}
