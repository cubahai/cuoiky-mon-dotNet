using BanHangDienTu.Models.Entities;

namespace BanHangDienTu.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<Order?> FindByKeyAsync(string userId, Guid key);
    Task<Order?> GetAsync(string userId, int id);
    Task<(IReadOnlyList<Order> Orders, int Page, int TotalPages)> GetHistoryAsync(string userId, int page, int pageSize);
    Task<(int? OrderId, string? Error)> PlaceAsync(Order order, string fingerprint);
}
