using System.ComponentModel.DataAnnotations;

namespace BanHangDienTu.Areas.Admin.ViewModels
{
    public class UserEditViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        public string FullName { get; set; }

        public string? PhoneNumber { get; set; }

        // Bỏ trống nếu không muốn đổi mật khẩu
        public string? NewPassword { get; set; }
    }
}