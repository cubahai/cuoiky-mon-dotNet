namespace BanHangDienTu.Models
{
    public class OrderItemViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
    }

    public class OrderViewModel
    {
        public string OrderId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusBadgeClass { get; set; } = "bg-secondary";
        public decimal TotalAmount { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new();
    }
}
