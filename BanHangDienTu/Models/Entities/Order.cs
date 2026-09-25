using System.ComponentModel.DataAnnotations;
using BanHangDienTu.Models.Enums;

namespace BanHangDienTu.Models.Entities;

public class Order
{
    public int Id { get; set; }
    [MaxLength(40)] public string OrderNumber { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    [MaxLength(100)] public string RecipientName { get; set; } = string.Empty;
    [MaxLength(20)] public string PhoneNumber { get; set; } = string.Empty;
    [MaxLength(500)] public string Address { get; set; } = string.Empty;
    [MaxLength(1000)] public string? Note { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public Guid CheckoutKey { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
