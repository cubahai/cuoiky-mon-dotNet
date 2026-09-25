using System.Data;
using BanHangDienTu.Data;
using BanHangDienTu.Models.Entities;
using BanHangDienTu.Repositories.Interfaces;
using BanHangDienTu.Services;
using Microsoft.EntityFrameworkCore;

namespace BanHangDienTu.Repositories.Implementations;

public sealed class OrderRepository(ApplicationDbContext context) : IOrderRepository
{
    public Task<Order?> FindByKeyAsync(string userId, Guid key) => context.Orders.AsNoTracking()
        .SingleOrDefaultAsync(x => x.UserId == userId && x.CheckoutKey == key);

    public Task<Order?> GetAsync(string userId, int id) => context.Orders.AsNoTracking().Include(x => x.Items)
        .SingleOrDefaultAsync(x => x.UserId == userId && x.Id == id);

    public async Task<(IReadOnlyList<Order> Orders, int Page, int TotalPages)> GetHistoryAsync(string userId, int page, int pageSize)
    {
        var query = context.Orders.AsNoTracking().Where(x => x.UserId == userId);
        var count = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(count / (double)pageSize);
        page = Math.Clamp(page, 1, Math.Max(1, totalPages));
        var orders = await query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (orders, page, totalPages);
    }

    public async Task<(int? OrderId, string? Error)> PlaceAsync(Order order, string fingerprint)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var existing = await FindByKeyAsync(order.UserId, order.CheckoutKey);
        if (existing is not null) return (existing.Id, null);

        var cart = await context.CartItems.Include(x => x.Product).ThenInclude(x => x.Category)
            .Where(x => x.UserId == order.UserId).OrderBy(x => x.ProductId).ToListAsync();
        if (cart.Count == 0) return (null, "Giỏ hàng đang trống.");
        if (CartSnapshot.Fingerprint(cart) != fingerprint)
            return (null, "Giỏ hàng hoặc giá sản phẩm đã thay đổi. Vui lòng xem lại và xác nhận đơn hàng.");
        foreach (var item in cart)
        {
            var product = item.Product;
            if (!product.IsActive || !product.Category.IsActive || product.Price < 0)
                return (null, $"Sản phẩm {product.Name} không còn được bán. Vui lòng cập nhật giỏ hàng.");
            if (item.Quantity < 1 || item.Quantity > 999 || item.Quantity > product.StockQuantity)
                return (null, $"Sản phẩm {product.Name} không đủ số lượng. Vui lòng cập nhật giỏ hàng.");

            // Conditional SQL update prevents overselling even with simultaneous checkouts.
            var changed = await context.Products.Where(x => x.Id == item.ProductId && x.StockQuantity >= item.Quantity)
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.StockQuantity, x => x.StockQuantity - item.Quantity));
            if (changed != 1) return (null, "Tồn kho vừa thay đổi. Vui lòng kiểm tra lại giỏ hàng.");
            order.Items.Add(new OrderItem
            {
                ProductId = product.Id, ProductName = product.Name, ImageUrl = product.ImageUrl,
                UnitPrice = product.Price, Quantity = item.Quantity
            });
        }
        order.TotalAmount = order.Items.Sum(x => x.UnitPrice * x.Quantity);
        context.Orders.Add(order);
        context.CartItems.RemoveRange(cart);
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        return (order.Id, null);
    }
}
