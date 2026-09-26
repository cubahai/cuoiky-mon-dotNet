using System.ComponentModel.DataAnnotations;

namespace BanHangDienTu.Areas.Admin.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá tiền")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng tồn kho")]
        public int StockQuantity { get; set; }

        public string? Note { get; set; }

        public string? ImageUrl { get; set; }
    }
}