namespace BanHangDienTu.ViewModels.Product;

public sealed class ProductDetailViewModel
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public int StockQuantity { get; init; }

    public string? Description { get; init; }

    public string? ImageUrl { get; init; }

    public int CategoryId { get; init; }

    public string CategoryName { get; init; } = string.Empty;

    public bool InStock => StockQuantity > 0;
}