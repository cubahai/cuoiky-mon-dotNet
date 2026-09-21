using BanHangDienTu.Models.Entities;
using BanHangDienTu.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BanHangDienTu.Repositories.Interfaces;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetFeaturedProductsAsync(int limit);

    Task<IReadOnlyList<Product>> GetNewestProductsAsync(int limit);

    Task<IReadOnlyList<Product>> SearchAsync(
        string keyword,
        int limit);

    Task<(IReadOnlyList<Product> Items, int TotalCount)>
        GetPagedAsync(
            string? searchTerm,
            int? categoryId,
            ProductSortOption sort,
            int page,
            int pageSize);

    Task<Product?> GetActiveByIdAsync(int id);
}