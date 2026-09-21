using BanHangDienTu.Models.Enums;
using BanHangDienTu.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BanHangDienTu.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? searchTerm,
        int? categoryId,
        ProductSortOption sort = ProductSortOption.Newest,
        int page = 1)
    {
        var model = await _productService.GetProductListAsync(
            searchTerm,
            categoryId,
            sort,
            page);

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var model = await _productService.GetProductDetailAsync(id);

        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }
}