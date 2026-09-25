namespace BanHangDienTu.ViewModels.Order;

public sealed class OrderItemViewModel
{
    public string ProductName { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice => UnitPrice * Quantity;
}
