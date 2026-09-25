using System.ComponentModel.DataAnnotations;

namespace BanHangDienTu.Models.Enums;

public enum PaymentStatus
{
    [Display(Name = "Chưa thanh toán")] Unpaid = 0,
    [Display(Name = "Đã thanh toán")] Paid = 1
}
