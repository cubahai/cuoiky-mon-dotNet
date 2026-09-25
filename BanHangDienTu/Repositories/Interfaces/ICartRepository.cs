using BanHangDienTu.Models.Entities;

namespace BanHangDienTu.Repositories.Interfaces;

public interface ICartRepository
{
    Task<IReadOnlyList<CartItem>> GetAsync(string userId);
    Task<string?> SetQuantityAsync(string userId, int productId, int quantity, bool increment);
    Task RemoveAsync(string userId, int productId);
}
