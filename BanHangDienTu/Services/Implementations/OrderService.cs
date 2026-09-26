using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BanHangDienTu.Data;
using BanHangDienTu.Models;
using BanHangDienTu.Models.Constants;
using BanHangDienTu.Services.Interfaces;
using BanHangDienTu.ViewModels.Cart;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BanHangDienTu.Services.Implementations;

public sealed class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        ApplicationDbContext context,
        ILogger<OrderService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(bool Succeeded, int OrderId, string? ErrorMessage)> CreateOrderAsync(
        string? userId,
        CheckoutViewModel model,
        List<CartItemViewModel> cartItems)
    {
        if (cartItems == null || cartItems.Count == 0)
        {
            return (false, 0, "Giỏ hàng của bạn đang trống.");
        }

        // Bước 1: Fetch và Kiểm tra tồn kho bên ngoài trước khi mở transaction
        var productIds = cartItems.Select(x => x.ProductId).Distinct().ToList();
        var productsInDb = await _context.Products
            .Where(p => productIds.Contains(p.Id) && p.IsActive)
            .ToDictionaryAsync(p => p.Id);

        decimal calculatedTotal = 0m;
        foreach (var cartItem in cartItems)
        {
            if (!productsInDb.TryGetValue(cartItem.ProductId, out var product))
            {
                return (false, 0, $"Sản phẩm '{cartItem.ProductName}' không còn tồn tại hoặc đã ngừng kinh doanh.");
            }

            if (product.StockQuantity < cartItem.Quantity)
            {
                return (false, 0, $"Sản phẩm '{product.Name}' chỉ còn {product.StockQuantity} trong kho, không đủ đáp ứng số lượng bạn đặt ({cartItem.Quantity}).");
            }

            calculatedTotal += product.Price * cartItem.Quantity;
        }

        // Bước 2: Mở Database Transaction ngắn gọn để ghi dữ liệu & cập nhật kho
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var newOrder = new Order
            {
                UserId = userId,
                CustomerName = model.CustomerName.Trim(),
                PhoneNumber = model.PhoneNumber.Trim(),
                ShippingAddress = model.ShippingAddress.Trim(),
                Note = model.Note?.Trim(),
                PaymentMethod = string.IsNullOrWhiteSpace(model.PaymentMethod) ? PaymentMethodConstants.Cod : model.PaymentMethod,
                OrderDate = DateTime.UtcNow,
                TotalAmount = calculatedTotal,
                Status = OrderStatus.Pending
            };

            await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync();

            foreach (var cartItem in cartItems)
            {
                var product = productsInDb[cartItem.ProductId];

                var detail = new OrderDetail
                {
                    OrderId = newOrder.Id,
                    ProductId = product.Id,
                    Quantity = cartItem.Quantity,
                    UnitPrice = product.Price
                };

                await _context.OrderDetails.AddAsync(detail);

                // Trừ số lượng tồn kho
                product.StockQuantity -= cartItem.Quantity;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Tạo đơn hàng #{OrderId} thành công cho khách hàng '{CustomerName}', Tổng tiền: {TotalAmount}",
                newOrder.Id, newOrder.CustomerName, calculatedTotal);

            return (true, newOrder.Id, null);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Lỗi xảy ra khi lưu đơn hàng cho người dùng {UserId}", userId);
            return (false, 0, "Không thể xử lý đơn hàng do sự cố hệ thống. Vui lòng thử lại sau.");
        }
    }

    public async Task<List<OrderHistoryViewModel>> GetOrderHistoryAsync(string userId, string? statusFilter = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return new List<OrderHistoryViewModel>();
        }

        var query = _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId);

        if (!string.IsNullOrWhiteSpace(statusFilter) && statusFilter != "Tất cả")
        {
            query = query.Where(o => o.Status == statusFilter);
        }

        var orders = await query
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
            .OrderByDescending(o => o.OrderDate)
            .ThenByDescending(o => o.Id)
            .ToListAsync();

        return orders.Select(MapToViewModel).ToList();
    }

    public async Task<OrderHistoryViewModel?> GetOrderDetailAsync(int orderId, string? userId = null)
    {
        if (orderId <= 0)
        {
            return null;
        }

        var query = _context.Orders
            .AsNoTracking()
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
            .Where(o => o.Id == orderId);

        if (!string.IsNullOrWhiteSpace(userId))
        {
            query = query.Where(o => o.UserId == userId);
        }

        var order = await query.FirstOrDefaultAsync();
        return order is null ? null : MapToViewModel(order);
    }

    public async Task<(bool Succeeded, string? ErrorMessage)> CancelOrderAsync(int orderId, string userId)
    {
        if (orderId <= 0 || string.IsNullOrWhiteSpace(userId))
        {
            return (false, "Yêu cầu không hợp lệ.");
        }

        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order is null)
        {
            return (false, "Không tìm thấy đơn hàng cần hủy.");
        }

        if (order.Status != OrderStatus.Pending)
        {
            return (false, $"Chỉ có thể hủy đơn hàng khi đang ở trạng thái '{OrderStatus.Pending}'. Đơn hàng hiện tại đang '{order.Status}'.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            order.Status = OrderStatus.Cancelled;

            // Hoàn lại số lượng tồn kho cho các sản phẩm
            var productIds = order.OrderDetails.Select(od => od.ProductId).Distinct().ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            foreach (var detail in order.OrderDetails)
            {
                if (products.TryGetValue(detail.ProductId, out var product))
                {
                    product.StockQuantity += detail.Quantity;
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Hủy đơn hàng #{OrderId} thành công bởi khách hàng {UserId}", orderId, userId);
            return (true, null);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Lỗi xảy ra khi hủy đơn hàng #{OrderId}", orderId);
            return (false, "Không thể hủy đơn hàng do lỗi hệ thống.");
        }
    }

    private static OrderHistoryViewModel MapToViewModel(Order order)
    {
        return new OrderHistoryViewModel
        {
            Id = order.Id,
            OrderId = order.Id.ToString(),
            CustomerName = order.CustomerName,
            PhoneNumber = order.PhoneNumber,
            ShippingAddress = order.ShippingAddress,
            Note = order.Note,
            PaymentMethod = PaymentMethodConstants.ToDisplayName(order.PaymentMethod),
            OrderDate = order.OrderDate,
            Status = order.Status,
            StatusBadgeClass = GetStatusBadgeClass(order.Status),
            TotalAmount = order.TotalAmount,
            Items = order.OrderDetails.Select(detail => new CartItemViewModel
            {
                ProductId = detail.ProductId,
                ProductName = detail.Product?.Name ?? "Sản phẩm không xác định",
                ImageUrl = string.IsNullOrWhiteSpace(detail.Product?.ImageUrl) ? "/images/default-product.svg" : detail.Product.ImageUrl,
                Quantity = detail.Quantity,
                Price = detail.UnitPrice
            }).ToList()
        };
    }

    private static string GetStatusBadgeClass(string status) => status switch
    {
        OrderStatus.Pending => "bg-warning text-dark",
        OrderStatus.Shipping => "bg-primary text-white",
        OrderStatus.Completed => "bg-success text-white",
        OrderStatus.Cancelled => "bg-danger text-white",
        _ => "bg-secondary text-white"
    };
}
