using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BanHangDienTu.Data;
using BanHangDienTu.Models;
using BanHangDienTu.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BanHangDienTu.Services.Implementations;

public sealed class CartService : ICartService
{
    private const string CartSessionKey = "BAN_HANG_DIEN_TU_CART";
    private const string BuyNowSessionKey = "BAN_HANG_DIEN_TU_BUY_NOW";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CartService> _logger;

    public CartService(
        IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext context,
        ILogger<CartService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _logger = logger;
    }

    private ISession? Session => _httpContextAccessor.HttpContext?.Session;

    public List<CartItemViewModel> GetCart()
    {
        if (Session is null)
        {
            return new List<CartItemViewModel>();
        }

        var sessionData = Session.GetString(CartSessionKey);
        if (string.IsNullOrWhiteSpace(sessionData))
        {
            return new List<CartItemViewModel>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<CartItemViewModel>>(sessionData) ?? new List<CartItemViewModel>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Lỗi giải mã JSON dữ liệu giỏ hàng từ Session");
            return new List<CartItemViewModel>();
        }
    }

    public async Task<(bool Succeeded, string? ErrorMessage)> AddToCartAsync(int productId, int quantity = 1)
    {
        if (productId <= 0 || quantity <= 0)
        {
            return (false, "Sản phẩm hoặc số lượng không hợp lệ.");
        }

        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);

        if (product is null)
        {
            return (false, "Sản phẩm không tồn tại hoặc đã ngừng kinh doanh.");
        }

        if (product.StockQuantity <= 0)
        {
            return (false, "Sản phẩm hiện đã hết hàng.");
        }

        var cart = GetCart();
        var existingItem = cart.FirstOrDefault(x => x.ProductId == productId);

        var requestedTotalQuantity = (existingItem?.Quantity ?? 0) + quantity;
        if (requestedTotalQuantity > product.StockQuantity)
        {
            return (false, $"Kho hàng chỉ còn {product.StockQuantity} sản phẩm, bạn không thể thêm vượt quá số lượng này.");
        }

        if (existingItem is not null)
        {
            existingItem.Quantity = requestedTotalQuantity;
        }
        else
        {
            cart.Add(new CartItemViewModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ImageUrl = string.IsNullOrWhiteSpace(product.ImageUrl) ? "/images/default-product.svg" : product.ImageUrl,
                Price = product.Price,
                Quantity = quantity
            });
        }

        SaveCart(cart);
        _logger.LogInformation("Đã thêm sản phẩm {ProductId} x {Quantity} vào giỏ hàng", productId, quantity);
        return (true, null);
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        if (productId <= 0)
        {
            return;
        }

        var cart = GetCart();
        var item = cart.FirstOrDefault(x => x.ProductId == productId);
        if (item is null)
        {
            return;
        }

        if (quantity <= 0)
        {
            cart.Remove(item);
        }
        else
        {
            item.Quantity = quantity;
        }

        SaveCart(cart);
    }

    public void RemoveItem(int productId)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(x => x.ProductId == productId);
        if (item is not null)
        {
            cart.Remove(item);
            SaveCart(cart);
            _logger.LogInformation("Đã xóa sản phẩm {ProductId} khỏi giỏ hàng", productId);
        }
    }

    public void ClearCart()
    {
        if (Session is not null)
        {
            Session.Remove(CartSessionKey);
            _logger.LogInformation("Đã xóa trắng giỏ hàng");
        }
    }

    public int GetTotalCount()
    {
        return GetCart().Sum(x => x.Quantity);
    }

    public decimal GetGrandTotal()
    {
        return GetCart().Sum(x => x.TotalPrice);
    }

    public async Task<(bool Succeeded, string? ErrorMessage)> SetBuyNowItemAsync(int productId, int quantity = 1)
    {
        if (quantity < 1)
        {
            return (false, "Số lượng đặt mua phải lớn hơn hoặc bằng 1.");
        }

        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);

        if (product is null)
        {
            return (false, "Sản phẩm không tồn tại hoặc đã ngừng kinh doanh.");
        }

        if (product.StockQuantity < quantity)
        {
            return (false, $"Kho hàng chỉ còn {product.StockQuantity} sản phẩm, không đủ số lượng bạn yêu cầu.");
        }

        var buyNowItem = new CartItemViewModel
        {
            ProductId = product.Id,
            ProductName = product.Name,
            ImageUrl = string.IsNullOrWhiteSpace(product.ImageUrl) ? "/images/default-product.svg" : product.ImageUrl,
            Price = product.Price,
            Quantity = quantity
        };

        if (Session is not null)
        {
            var json = JsonSerializer.Serialize(buyNowItem);
            Session.SetString(BuyNowSessionKey, json);
            _logger.LogInformation("Đã lưu phiên mua ngay cho sản phẩm {ProductId} x {Quantity}", productId, quantity);
        }

        return (true, null);
    }

    public CartItemViewModel? GetBuyNowItem()
    {
        if (Session is null)
        {
            return null;
        }

        var sessionData = Session.GetString(BuyNowSessionKey);
        if (string.IsNullOrWhiteSpace(sessionData))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<CartItemViewModel>(sessionData);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Lỗi giải mã session mua ngay");
            return null;
        }
    }

    public void ClearBuyNowItem()
    {
        if (Session is not null)
        {
            Session.Remove(BuyNowSessionKey);
            _logger.LogInformation("Đã xóa phiên mua ngay");
        }
    }

    private void SaveCart(List<CartItemViewModel> cart)
    {
        if (Session is null)
        {
            return;
        }

        var json = JsonSerializer.Serialize(cart);
        Session.SetString(CartSessionKey, json);
    }
}
