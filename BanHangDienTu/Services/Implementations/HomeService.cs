using BanHangDienTu.Models.Entities;
using BanHangDienTu.Repositories.Interfaces;
using BanHangDienTu.Services.Interfaces;
using BanHangDienTu.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BanHangDienTu.Services.Implementations;

public sealed class HomeService : IHomeService
{
    private const int FeaturedProductLimit = 8;
    private const int NewProductLimit = 8;
    private const int SearchResultLimit = 12;

    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public HomeService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<HomeViewModel> GetHomeDataAsync(
        string? searchTerm = null)
    {
        var featuredProducts =
            await _productRepository.GetFeaturedProductsAsync(
                FeaturedProductLimit);

        var newProducts =
            await _productRepository.GetNewestProductsAsync(
                NewProductLimit);

        var categories =
            await _categoryRepository.GetActiveCategoriesAsync();

        var normalizedSearchTerm = searchTerm?.Trim();

        IReadOnlyList<Product> searchResults =
            Array.Empty<Product>();

        if (!string.IsNullOrWhiteSpace(normalizedSearchTerm))
        {
            searchResults =
                await _productRepository.SearchAsync(
                    normalizedSearchTerm,
                    SearchResultLimit);
        }

        return new HomeViewModel
        {
            SearchTerm = normalizedSearchTerm,

            FeaturedProducts = featuredProducts
                .Select(MapProduct)
                .ToList(),

            NewProducts = newProducts
                .Select(MapProduct)
                .ToList(),

            Categories = categories
                .Select(c => new HomeCategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList(),

            SearchResults = searchResults
                .Select(MapProduct)
                .ToList()
        };
    }

    private static HomeProductViewModel MapProduct(Product product)
    {
        return new HomeProductViewModel
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