using BanHangDienTu.Models.Enums;
using System;
using System.Collections.Generic;

namespace BanHangDienTu.ViewModels.Product;

public sealed class ProductListViewModel
{
    public IReadOnlyList<ProductListItemViewModel> Products { get; init; }
        = Array.Empty<ProductListItemViewModel>();

    public IReadOnlyList<ProductCategoryFilterViewModel> Categories { get; init; }
        = Array.Empty<ProductCategoryFilterViewModel>();

    public string? SearchTerm { get; init; }

    public int? CategoryId { get; init; }

    public ProductSortOption Sort { get; init; } = ProductSortOption.Newest;

    public int CurrentPage { get; init; } = 1;

    public int PageSize { get; init; } = 12;

    public int TotalItems { get; init; }

    public int TotalPages =>
        TotalItems == 0
            ? 0
            : (int)Math.Ceiling(TotalItems / (double)PageSize);

    public bool HasPreviousPage => CurrentPage > 1;

    public bool HasNextPage => CurrentPage < TotalPages;
}