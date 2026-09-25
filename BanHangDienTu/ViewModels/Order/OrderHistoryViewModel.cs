namespace BanHangDienTu.ViewModels.Order;

public sealed class OrderHistoryViewModel
{
    public IReadOnlyList<OrderSummaryViewModel> Orders { get; init; } = [];
    public int CurrentPage { get; init; }
    public int TotalPages { get; init; }
}
