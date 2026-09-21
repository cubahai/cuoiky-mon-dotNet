using System.ComponentModel.DataAnnotations;

namespace BanHangDienTu.ViewModels.Account;

public sealed class ProfileViewModel
{
    [Required(ErrorMessage = "Họ tên không được để trống")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "Họ tên phải từ 2 đến 100 ký tự")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(
        20,
        ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
    [Display(Name = "Số điện thoại")]
    public string? PhoneNumber { get; set; }

    [StringLength(
        500,
        ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
    [Display(Name = "Địa chỉ giao hàng")]
    public string? Address { get; set; }
}