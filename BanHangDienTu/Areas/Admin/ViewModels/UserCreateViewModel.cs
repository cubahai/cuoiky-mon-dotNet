using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security;

namespace BanHangDienTu.Areas.Admin.ViewModels
{
    public class UserCreateViewModel 
    {
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không chính xác")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Vui lòng nhập mật khẩu")]
        [MinLength(6, ErrorMessage ="Mật khẩu ít nhất phải có 9 ký tự")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản")]
        public String FullName { get; set; }
        public string PhoneNumber { get; set; }
           
        }
    }

