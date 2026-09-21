using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BanHangDienTu.Models.Entities;

public class ApplicationUser : IdentityUser
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Address { get; set; }
}