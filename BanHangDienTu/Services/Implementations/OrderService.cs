using System.Security.Cryptography;
using System.Text.Json;
using BanHangDienTu.Models.Entities;
using BanHangDienTu.Models.Enums;
using BanHangDienTu.Repositories.Interfaces;
using BanHangDienTu.Services.Interfaces;
using BanHangDienTu.ViewModels.Checkout;
using BanHangDienTu.ViewModels.Order;
using Microsoft.AspNetCore.DataProtection;

namespace BanHangDienTu.Services.Implementations;

public sealed class OrderService : IOrderService
{
    private readonly ICartRepository _cart;
    private readonly IOrderRepository _orders;
    private readonly IAccountService _accounts;
    private readonly IDataProtector _protector;

    public OrderService(ICartRepository cart, IOrderRepository orders, IAccountService accounts, IDataProtectionProvider protection)
    {
        _cart = cart; _orders = orders; _accounts = accounts;
        _protector = protection.CreateProtector("BanHangDienTu.Checkout.v1");
    }

    private sealed record Confirmation(string UserId, Guid Key, string Fingerprint, DateTime ExpiresAt);

    public async Task<CheckoutViewModel> GetCheckoutAsync(string userId)
    {
        var items = await _cart.GetAsync(userId);
        var profile = await _accounts.GetProfileAsync(userId);
        var confirmation = new Confirmation(userId, Guid.NewGuid(), CartSnapshot.Fingerprint(items), DateTime.UtcNow.AddMinutes(30));
        return new CheckoutViewModel
        {
            RecipientName = profile?.FullName ?? string.Empty, PhoneNumber = profile?.PhoneNumber ?? string.Empty,
            Address = profile?.Address ?? string.Empty, Cart = CartService.Map(items),
            CheckoutToken = _protector.Protect(JsonSerializer.Serialize(confirmation))
        };
    }

    public async Task<(int? OrderId, string? Error)> PlaceAsync(string userId, CheckoutViewModel model)
    {
        Confirmation? confirmation;
        try { confirmation = JsonSerializer.Deserialize<Confirmation>(_protector.Unprotect(model.CheckoutToken)); }
        catch (Exception ex) when (ex is CryptographicException or JsonException or ArgumentException)
        { return (null, "Phiên đặt hàng không hợp lệ. Vui lòng xem lại đơn hàng và xác nhận."); }
        if (confirmation is null || confirmation.UserId != userId)
            return (null, "Phiên đặt hàng không hợp lệ.");
        var existing = await _orders.FindByKeyAsync(userId, confirmation.Key);
        if (existing is not null) return (existing.Id, null);
        if (confirmation.ExpiresAt < DateTime.UtcNow)
            return (null, "Phiên đặt hàng đã hết hạn. Vui lòng xem lại và xác nhận.");
        if (string.IsNullOrWhiteSpace(model.RecipientName) || model.RecipientName.Trim().Length < 2 ||
            string.IsNullOrWhiteSpace(model.PhoneNumber) || string.IsNullOrWhiteSpace(model.Address))
            return (null, "Vui lòng điền đầy đủ thông tin người nhận.");

        var order = new Order
        {
            UserId = userId, CheckoutKey = confirmation.Key, OrderNumber = $"DH-{confirmation.Key:N}",
            CreatedAt = DateTime.UtcNow, RecipientName = model.RecipientName.Trim(),
            PhoneNumber = model.PhoneNumber.Trim(), Address = model.Address.Trim(), Note = model.Note?.Trim(),
            Status = OrderStatus.Pending, PaymentMethod = PaymentMethod.Cod, PaymentStatus = PaymentStatus.Unpaid
        };
        try { return await _orders.PlaceAsync(order, confirmation.Fingerprint); }
        catch (Exception ex) when (CartService.IsConflict(ex))
        {
            existing = await _orders.FindByKeyAsync(userId, confirmation.Key);
            return existing is not null ? (existing.Id, null) : (null, "Giỏ hàng hoặc tồn kho vừa được cập nhật. Vui lòng xem lại và xác nhận.");
        }
    }

    public async Task<OrderHistoryViewModel> GetHistoryAsync(string userId, int page)
    {
        var result = await _orders.GetHistoryAsync(userId, page, 10);
        return new OrderHistoryViewModel
        {
            CurrentPage = result.Page, TotalPages = result.TotalPages,
            Orders = result.Orders.Select(x => new OrderSummaryViewModel
            {
                Id = x.Id, OrderNumber = x.OrderNumber, CreatedAt = x.CreatedAt,
                TotalAmount = x.TotalAmount, Status = x.Status, PaymentStatus = x.PaymentStatus
            }).ToList()
        };
    }

    public async Task<OrderDetailViewModel?> GetDetailAsync(string userId, int id)
    {
        var order = await _orders.GetAsync(userId, id);
        return order is null ? null : new OrderDetailViewModel
        {
            Id = order.Id, OrderNumber = order.OrderNumber, CreatedAt = order.CreatedAt,
            TotalAmount = order.TotalAmount, Status = order.Status, PaymentStatus = order.PaymentStatus,
            RecipientName = order.RecipientName, PhoneNumber = order.PhoneNumber, Address = order.Address, Note = order.Note,
            Items = order.Items.OrderBy(x => x.Id).Select(x => new OrderItemViewModel
            {
                ProductName = x.ProductName, ImageUrl = x.ImageUrl, Quantity = x.Quantity, UnitPrice = x.UnitPrice
            }).ToList()
        };
    }
}
