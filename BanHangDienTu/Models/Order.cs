using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BanHangDienTu.Models.Constants;

namespace BanHangDienTu.Models;

public class Order
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
    [StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
    [StringLength(500)]
    public string ShippingAddress { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Note { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = OrderStatus.Pending;

    [StringLength(50)]
    public string PaymentMethod { get; set; } = PaymentMethodConstants.Cod;

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}