using BanHangDienTu.Areas.Admin.ViewModels;
using BanHangDienTu.Data;
using BanHangDienTu.Models;
using BanHangDienTu.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BanHangDienTu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Khởi tạo và kết nối Database
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Hiển thị danh sách sản phẩm
        public IActionResult Index()
        {
            var products = _context.Products
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Note = p.Note,
                    ImageUrl = p.ImageUrl
                }).ToList();

            return View(products);
        }

        // 2. Hiển thị Form thêm mới
        [HttpGet]
        public IActionResult Create()
        {
            return View(new ProductViewModel());
        }

        // 3. Xử lý lưu dữ liệu thêm mới
        [HttpPost]
        public IActionResult Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = model.Name,
                    Price = model.Price,
                    StockQuantity = model.StockQuantity,
                    Note = model.Note,
                    ImageUrl = model.ImageUrl ?? "/images/default-product.png",
                    IsActive = true
                };

                _context.Products.Add(product);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // 4. Hiển thị Form Sửa thông tin sản phẩm
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            var model = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                Note = product.Note,
                ImageUrl = product.ImageUrl
            };
            return View(model);
        }

        // 5. Xử lý lưu dữ liệu sau khi chỉnh sửa
        [HttpPost]
        public IActionResult Edit(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = _context.Products.Find(model.Id);
                if (product == null) return NotFound();

                product.Name = model.Name;
                product.Price = model.Price;
                product.StockQuantity = model.StockQuantity;
                product.Note = model.Note;

                if (!string.IsNullOrEmpty(model.ImageUrl))
                {
                    product.ImageUrl = model.ImageUrl;
                }

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // 6. Xử lý Xóa sản phẩm
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}