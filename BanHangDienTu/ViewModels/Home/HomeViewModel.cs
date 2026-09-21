using System;
using System.Collections.Generic;

namespace BanHangDienTu.ViewModels.Home;

public sealed class HomeViewModel
{
    public IReadOnlyList<HomeProductViewModel> FeaturedProducts { get; init; }
        = Array.Empty<HomeProductViewModel>();

    public IReadOnlyList<HomeProductViewModel> NewProducts { get; init; }
        = Array.Empty<HomeProductViewModel>();

    public IReadOnlyList<HomeCategoryViewModel> Categories { get; init; }
        = Array.Empty<HomeCategoryViewModel>();

    public IReadOnlyList<HomeProductViewModel> SearchResults { get; init; }
        = Array.Empty<HomeProductViewModel>();

    public string? SearchTerm { get; init; }

    public bool HasSearch =>
        !string.IsNullOrWhiteSpace(SearchTerm);
}