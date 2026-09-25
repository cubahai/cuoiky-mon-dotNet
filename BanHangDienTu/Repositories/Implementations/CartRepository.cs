using System.Data;
using BanHangDienTu.Data;
using BanHangDienTu.Models.Entities;
using BanHangDienTu.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BanHangDienTu.Repositories.Implementations;

public sealed class CartRepository(ApplicationDbContext context) : ICartRepository
{
    public async Task<IReadOnlyList<CartItem>> GetAsync(string userId) =>
        await context.CartItems.AsNoTracking().Include(x => x.Product).ThenInclude(x => x.Category)
            .Where(x => x.UserId == userId).OrderBy(x => x.ProductId).ToListAsync();

    public async Task<string?> SetQuantityAsync(string userId, int productId, int quantity, bool increment)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var item = await context.CartItems.SingleOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);
        if (!increment && item is null) return "Sản phẩm không còn trong giỏ hàng.";
        var product = await context.Products.Include(x => x.Category).SingleOrDefaultAsync(x => x.Id == productId);
        if (product is null || !product.IsActive || !product.Category.IsActive || product.Price < 0)
            return "Sản phẩm hiện không còn được bán.";
        var desired = (long)quantity + (increment ? item?.Quantity ?? 0 : 0);
        if (desired is < 1 or > 999) return "Số lượng mỗi sản phẩm phải từ 1 đến 999.";
        if (desired > product.StockQuantity) return $"Sản phẩm chỉ còn {product.StockQuantity} trong kho.";
        if (item is null)
            context.CartItems.Add(new CartItem { UserId = userId, ProductId = productId, Quantity = (int)desired });
        else item.Quantity = (int)desired;
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        return null;
    }

    public async Task RemoveAsync(string userId, int productId) =>
        await context.CartItems.Where(x => x.UserId == userId && x.ProductId == productId).ExecuteDeleteAsync();
}
