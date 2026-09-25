using System.ComponentModel.DataAnnotations;

namespace BanHangDienTu.Models.Enums;

public enum PaymentMethod
{
    [Display(Name = "Thanh toán khi nhận hàng (COD)")] Cod = 0
}
