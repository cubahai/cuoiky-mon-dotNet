namespace BanHangDienTu.ViewModels.Cart;

public sealed class CartViewModel
{
    public IReadOnlyList<CartItemViewModel> Items { get; init; } = [];
    public decimal TotalAmount => Items.Sum(x => x.TotalPrice);
    public bool CanCheckout => Items.Count > 0 && Items.All(x => x.CanOrder);
}
