using BanHangDienTu.Data;
using BanHangDienTu.Models.Entities;
using BanHangDienTu.Models.Enums;
using BanHangDienTu.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BanHangDienTu.Repositories.Implementations;

public sealed class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Product>> GetFeaturedProductsAsync(int limit)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p =>
                p.IsActive &&
                p.IsFeatured &&
                p.Category.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Product>> GetNewestProductsAsync(int limit)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p =>
                p.IsActive &&
                p.Category.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Product>> SearchAsync(
        string keyword,
        int limit)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p =>
                p.IsActive &&
                p.Category.IsActive &&
                (p.Name.Contains(keyword) ||
                 p.Category.Name.Contains(keyword)))
            .OrderBy(p => p.Name)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<(IReadOnlyList<Product> Items, int TotalCount)>
        GetPagedAsync(
            string? searchTerm,
            int? categoryId,
            ProductSortOption sort,
            int page,
            int pageSize)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p =>
                p.IsActive &&
                p.Category.IsActive);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p =>
                p.Name.Contains(searchTerm) ||
                p.Category.Name.Contains(searchTerm));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p =>
                p.CategoryId == categoryId.Value);
        }

        var totalCount = await query.CountAsync();

        query = sort switch
        {
            ProductSortOption.PriceAscending =>
                query
                    .OrderBy(p => p.Price)
                    .ThenBy(p => p.Name),

            ProductSortOption.PriceDescending =>
                query
                    .OrderByDescending(p => p.Price)
                    .ThenBy(p => p.Name),

            ProductSortOption.NameAscending =>
                query.OrderBy(p => p.Name),

            _ =>
                query
                    .OrderByDescending(p => p.CreatedAt)
                    .ThenByDescending(p => p.Id)
        };

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Product?> GetActiveByIdAsync(int id)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p =>
                p.Id == id &&
                p.IsActive &&
                p.Category.IsActive);
    }
}