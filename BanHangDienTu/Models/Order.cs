using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BanHangDienTu.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string ShippingAddress { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        // Trạng thái: "Chờ xác nhận", "Đang giao", "Hoàn thành", "Đã hủy"
        public string Status { get; set; } = "Chờ xác nhận";

        // Liên kết 1-Nhiều với bảng OrderDetail
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}