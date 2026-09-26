using System.Collections.Generic;
using System.Threading.Tasks;
using BanHangDienTu.Models;

namespace BanHangDienTu.Services.Interfaces;

public interface ICartService
{
    List<CartItemViewModel> GetCart();

    Task<(bool Succeeded, string? ErrorMessage)> AddToCartAsync(int productId, int quantity = 1);

    void UpdateQuantity(int productId, int quantity);

    void RemoveItem(int productId);

    void ClearCart();

    int GetTotalCount();

    decimal GetGrandTotal();

    Task<(bool Succeeded, string? ErrorMessage)> SetBuyNowItemAsync(int productId, int quantity = 1);

    CartItemViewModel? GetBuyNowItem();

    void ClearBuyNowItem();
}
