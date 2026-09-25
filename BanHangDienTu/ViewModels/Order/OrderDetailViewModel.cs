namespace BanHangDienTu.ViewModels.Order;

public sealed class OrderDetailViewModel : OrderSummaryViewModel
{
    public string RecipientName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string? Note { get; init; }
    public IReadOnlyList<OrderItemViewModel> Items { get; init; } = [];
}
