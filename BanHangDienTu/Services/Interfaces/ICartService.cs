using BanHangDienTu.ViewModels.Cart;

namespace BanHangDienTu.Services.Interfaces;

public interface ICartService
{
    Task<CartViewModel> GetAsync(string userId);
    Task<string?> AddAsync(string userId, int productId, int quantity);
    Task<string?> UpdateAsync(string userId, int productId, int quantity);
    Task RemoveAsync(string userId, int productId);
}
