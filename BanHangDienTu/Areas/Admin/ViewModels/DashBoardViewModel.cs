namespace BanHangDienTu.Areas.Admin.ViewModels
{
    public class DashBoardViewModel
    {
        public int TotalProducts { get; set; }
        public int PendingOrders { get; set; } 
        public decimal MonthlyRevenue { get; set; }
        public int LowStockCount { get; set; }
    }
}
