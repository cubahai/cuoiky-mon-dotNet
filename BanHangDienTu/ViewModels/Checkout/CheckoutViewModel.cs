using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using BanHangDienTu.ViewModels.Cart;

namespace BanHangDienTu.ViewModels.Checkout;

public sealed class CheckoutViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên người nhận.")]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Người nhận")] public string RecipientName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [StringLength(20)]
    [Display(Name = "Số điện thoại")] public string PhoneNumber { get; set; } = string.Empty;
    [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng.")]
    [StringLength(500)]
    [Display(Name = "Địa chỉ nhận hàng")] public string Address { get; set; } = string.Empty;
    [StringLength(1000)]
    [Display(Name = "Ghi chú")] public string? Note { get; set; }
    [Required] public string CheckoutToken { get; set; } = string.Empty;
    [BindNever] public CartViewModel Cart { get; set; } = new();
}
