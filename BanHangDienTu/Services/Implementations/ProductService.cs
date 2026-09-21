using BanHangDienTu.Models.Entities;
using BanHangDienTu.Models.Enums;
using BanHangDienTu.Repositories.Interfaces;
using BanHangDienTu.Services.Interfaces;
using BanHangDienTu.ViewModels.Product;
using System;
using System.Threading.Tasks;

namespace BanHangDienTu.Services.Implementations;

public sealed class ProductService : IProductService
{
    private const int PageSize = 12;
    private const int MaxSearchTermLength = 100;

    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductListViewModel> GetProductListAsync(
        string? searchTerm,
        int? categoryId,
        ProductSortOption sort,
        int page)
    {
        var normalizedSearchTerm = NormalizeSearchTerm(searchTerm);

        if (categoryId <= 0)
        {
            categoryId = null;
        }

        if (!Enum.IsDefined(typeof(ProductSortOption), sort))
        {
            sort = ProductSortOption.Newest;
        }

        if (page < 1)
        {
            page = 1;
        }

        var categories =
            await _categoryRepository.GetActiveCategoriesAsync();

        var result =
            await _productRepository.GetPagedAsync(
                normalizedSearchTerm,
                categoryId,
                sort,
                page,
                PageSize);

        var totalPages = result.TotalCount == 0
            ? 0
            : (int)Math.Ceiling(
                result.TotalCount / (double)PageSize);

        if (totalPages > 0 && page > totalPages)
        {
            page = totalPages;

            result =
                await _productRepository.GetPagedAsync(
                    normalizedSearchTerm,
                    categoryId,
                    sort,
                    page,
                    PageSize);
        }

        return new ProductListViewModel
        {
            SearchTerm = normalizedSearchTerm,
            CategoryId = categoryId,
            Sort = sort,
            CurrentPage = page,
            PageSize = PageSize,
            TotalItems = result.TotalCount,

            Categories = categories
                .Select(c => new ProductCategoryFilterViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList(),

            Products = result.Items
                .Select(MapProduct)
                .ToList()
        };
    }

    public async Task<ProductDetailViewModel?> GetProductDetailAsync(int id)
    {
        if (id <= 0)
        {
            return null;
        }

        var product = await _productRepository.GetActiveByIdAsync(id);

        if (product is null)
        {
            return null;
        }

        return new ProductDetailViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Description = product.Note,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name
        };
    }

    private static string? NormalizeSearchTerm(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return null;
        }

        var normalized = searchTerm.Trim();

        if (normalized.Length > MaxSearchTermLength)
        {
            normalized = normalized[..MaxSearchTermLength];
        }

        return normalized;
    }

    private static ProductListItemViewModel MapProduct(Product product)
    {
        return new ProductListItemViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            CategoryName = product.Category.Name,
            StockQuantity = product.StockQuantity
        };
    }
}