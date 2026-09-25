using System.Security.Cryptography;
using System.Text.Json;
using BanHangDienTu.Models.Entities;

namespace BanHangDienTu.Services;

public static class CartSnapshot
{
    // Stable across requests; binds the confirmation to the exact cart and prices displayed.
    public static string Fingerprint(IEnumerable<CartItem> items) =>
        Convert.ToHexString(SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(
            items.OrderBy(x => x.ProductId).Select(x => new { x.ProductId, x.Quantity, x.Product.Price }))));
}
