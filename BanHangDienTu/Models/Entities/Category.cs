using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BanHangDienTu.Models.Entities;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên danh mục không được để trống")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}