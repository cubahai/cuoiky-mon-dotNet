namespace BanHangDienTu.ViewModels.Product;

public sealed class ProductListItemViewModel
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public string? ImageUrl { get; init; }

    public string CategoryName { get; init; } = string.Empty;

    public int StockQuantity { get; init; }
}