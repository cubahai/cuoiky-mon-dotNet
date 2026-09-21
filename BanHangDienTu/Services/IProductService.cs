using BanHangDienTu.Models.Enums;
using BanHangDienTu.ViewModels.Product;
using System.Threading.Tasks;

namespace BanHangDienTu.Services.Interfaces;

public interface IProductService
{
    Task<ProductListViewModel> GetProductListAsync(
        string? searchTerm,
        int? categoryId,
        ProductSortOption sort,
        int page);

    Task<ProductDetailViewModel?> GetProductDetailAsync(int id);
}