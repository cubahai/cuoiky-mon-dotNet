using System.Collections.Generic;
using System.Threading.Tasks;
using BanHangDienTu.Models;
using BanHangDienTu.ViewModels.Cart;

namespace BanHangDienTu.Services.Interfaces;

public interface IOrderService
{
    Task<(bool Succeeded, int OrderId, string? ErrorMessage)> CreateOrderAsync(
        string? userId,
        CheckoutViewModel model,
        List<CartItemViewModel> cartItems);

    Task<List<OrderHistoryViewModel>> GetOrderHistoryAsync(string userId, string? statusFilter = null);

    Task<OrderHistoryViewModel?> GetOrderDetailAsync(int orderId, string? userId = null);

    Task<(bool Succeeded, string? ErrorMessage)> CancelOrderAsync(int orderId, string userId);
}
