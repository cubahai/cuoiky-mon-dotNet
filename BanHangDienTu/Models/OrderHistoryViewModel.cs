using System;
using System.Collections.Generic;

namespace BanHangDienTu.Models;

public class OrderHistoryViewModel
{
    public int Id { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusBadgeClass { get; set; } = "bg-secondary";
    public decimal TotalAmount { get; set; }
    public List<CartItemViewModel> Items { get; set; } = new();
}
