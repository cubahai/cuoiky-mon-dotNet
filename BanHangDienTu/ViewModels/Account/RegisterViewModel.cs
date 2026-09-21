using System.ComponentModel.DataAnnotations;

namespace BanHangDienTu.ViewModels.Account;

public sealed class RegisterViewModel
{
    [Required(ErrorMessage = "Họ tên không được để trống")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "Họ tên phải từ 2 đến 100 ký tự")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [StringLength(
        100,
        MinimumLength = 6,
        ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,100}$",
        ErrorMessage = "Mật khẩu phải có chữ hoa, chữ thường và chữ số")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
    [DataType(DataType.Password)]
    [Compare(
        nameof(Password),
        ErrorMessage = "Mật khẩu xác nhận không khớp")]
    [Display(Name = "Xác nhận mật khẩu")]
    public string ConfirmPassword { get; set; } = string.Empty;
}