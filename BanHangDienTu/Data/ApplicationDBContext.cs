using BanHangDienTu.Models;
using Microsoft.EntityFrameworkCore;

namespace BanHangDienTu.Data
{

   public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Đại diện cho bảng Products trong database
        public DbSet<Product> Products { get; set; }

        // Sau này chúng ta sẽ thêm DbSet<User>, DbSet<Order> vào đây
    }
}
