namespace BanHangDienTu.ViewModels.Cart;

public sealed class CartItemViewModel
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public decimal Price { get; init; }
    public int Quantity { get; init; }
    public int StockQuantity { get; init; }
    public bool IsActive { get; init; }
    public bool CanOrder => IsActive && Price >= 0 && Quantity is >= 1 and <= 999 && Quantity <= StockQuantity;
    public decimal TotalPrice => Price * Quantity;
}
