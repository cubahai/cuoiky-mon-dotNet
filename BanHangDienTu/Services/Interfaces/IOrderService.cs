using BanHangDienTu.ViewModels.Checkout;
using BanHangDienTu.ViewModels.Order;

namespace BanHangDienTu.Services.Interfaces;

public interface IOrderService
{
    Task<CheckoutViewModel> GetCheckoutAsync(string userId);
    Task<(int? OrderId, string? Error)> PlaceAsync(string userId, CheckoutViewModel model);
    Task<OrderHistoryViewModel> GetHistoryAsync(string userId, int page);
    Task<OrderDetailViewModel?> GetDetailAsync(string userId, int id);
}
