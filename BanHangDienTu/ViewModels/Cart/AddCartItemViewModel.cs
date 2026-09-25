using System.ComponentModel.DataAnnotations;

namespace BanHangDienTu.ViewModels.Cart;

public sealed class AddCartItemViewModel
{
    [Range(1, int.MaxValue)] public int ProductId { get; set; }
    [Range(1, 999, ErrorMessage = "Số lượng phải từ 1 đến 999.")] public int Quantity { get; set; } = 1;
    public bool BuyNow { get; set; }
}
