using System.ComponentModel.DataAnnotations;

namespace BanHangDienTu.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        public string Note { get; set; }

        public string ImageUrl { get; set; }

        // Trạng thái: true = Còn kinh doanh, false = Ngừng kinh doanh
        public bool IsActive { get; set; } = true;
    }
}
