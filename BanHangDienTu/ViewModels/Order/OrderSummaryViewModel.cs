using BanHangDienTu.Models.Enums;

namespace BanHangDienTu.ViewModels.Order;

public class OrderSummaryViewModel
{
    public int Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public decimal TotalAmount { get; init; }
    public OrderStatus Status { get; init; }
    public PaymentStatus PaymentStatus { get; init; }
    public string StatusText => Status switch
    {
        OrderStatus.Pending => "Chờ xác nhận", OrderStatus.Confirmed => "Đã xác nhận",
        OrderStatus.Shipping => "Đang giao hàng", OrderStatus.Delivered => "Đã giao hàng",
        OrderStatus.Cancelled => "Đã hủy", _ => "Chưa xác định"
    };
    public string PaymentStatusText => PaymentStatus == PaymentStatus.Paid ? "Đã thanh toán" : "Chưa thanh toán";
}
