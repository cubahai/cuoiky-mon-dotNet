using BanHangDienTu.Models.Entities;
using BanHangDienTu.Repositories.Interfaces;
using BanHangDienTu.Services.Interfaces;
using BanHangDienTu.ViewModels.Cart;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BanHangDienTu.Services.Implementations;

public sealed class CartService(ICartRepository repository) : ICartService
{
    public async Task<CartViewModel> GetAsync(string userId) => Map(await repository.GetAsync(userId));
    public Task<string?> AddAsync(string userId, int productId, int quantity) => SetAsync(userId, productId, quantity, true);
    public Task<string?> UpdateAsync(string userId, int productId, int quantity) => SetAsync(userId, productId, quantity, false);
    public Task RemoveAsync(string userId, int productId) => repository.RemoveAsync(userId, productId);

    private async Task<string?> SetAsync(string userId, int productId, int quantity, bool increment)
    {
        if (productId <= 0 || quantity is < 1 or > 999) return "Sản phẩm hoặc số lượng không hợp lệ.";
        try { return await repository.SetQuantityAsync(userId, productId, quantity, increment); }
        catch (Exception ex) when (IsConflict(ex))
        { return "Giỏ hàng vừa được cập nhật ở yêu cầu khác. Vui lòng tải lại và thử lại."; }
    }

    internal static bool IsConflict(Exception ex) =>
        ex is SqlException { Number: 1205 or 2601 or 2627 } ||
        ex is DbUpdateException { InnerException: SqlException { Number: 1205 or 2601 or 2627 } };

    internal static CartViewModel Map(IReadOnlyList<CartItem> items) => new()
    {
        Items = items.Select(x => new CartItemViewModel
        {
            ProductId = x.ProductId, ProductName = x.Product.Name, ImageUrl = x.Product.ImageUrl,
            Price = x.Product.Price, Quantity = x.Quantity, StockQuantity = x.Product.StockQuantity,
            IsActive = x.Product.IsActive && x.Product.Category.IsActive
        }).ToList()
    };
}
