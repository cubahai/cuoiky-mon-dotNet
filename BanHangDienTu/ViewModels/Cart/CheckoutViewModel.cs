using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BanHangDienTu.Models;
using BanHangDienTu.Models.Constants;

namespace BanHangDienTu.ViewModels.Cart;

public class CheckoutViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
    [Display(Name = "Họ và tên người nhận")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
    [Display(Name = "Số điện thoại liên hệ")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng")]
    [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
    [Display(Name = "Địa chỉ nhận hàng")]
    public string ShippingAddress { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
    [Display(Name = "Ghi chú đơn hàng")]
    public string? Note { get; set; }

    [Display(Name = "Phương thức thanh toán")]
    public string PaymentMethod { get; set; } = PaymentMethodConstants.Cod;

    public List<CartItemViewModel> Items { get; set; } = new();

    public decimal GrandTotal { get; set; }

    public bool IsBuyNow { get; set; }
}
