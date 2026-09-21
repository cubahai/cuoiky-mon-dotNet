using BanHangDienTu.Models.Constants;
using BanHangDienTu.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BanHangDienTu.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var serviceProvider = scope.ServiceProvider;

        var context =
            serviceProvider.GetRequiredService<ApplicationDbContext>();

        var roleManager =
            serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var userManager =
            serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var configuration =
            serviceProvider.GetRequiredService<IConfiguration>();

        await SeedRolesAsync(roleManager);

        await SeedAdminAsync(
            userManager,
            configuration);

        await SeedCategoriesAsync(context);

        await SeedProductsAsync(context);
    }

    private static async Task SeedRolesAsync(
        RoleManager<IdentityRole> roleManager)
    {
        var roles = new[]
        {
            AppRoles.Admin,
            AppRoles.Customer
        };

        foreach (var roleName in roles)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(
                new IdentityRole(roleName));

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Không thể tạo role '{roleName}': {errors}");
            }
        }
    }

    private static async Task SeedAdminAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        var email =
            configuration["SeedAdmin:Email"];

        var password =
            configuration["SeedAdmin:Password"];

        var fullName =
            configuration["SeedAdmin:FullName"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(fullName))
        {
            throw new InvalidOperationException(
                "Thiếu cấu hình SeedAdmin trong User Secrets.");
        }

        var adminUser =
            await userManager.FindByEmailAsync(email);

        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true
            };

            var createResult =
                await userManager.CreateAsync(
                    adminUser,
                    password);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    createResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Không thể tạo tài khoản Admin: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(
                adminUser,
                AppRoles.Admin))
        {
            var roleResult =
                await userManager.AddToRoleAsync(
                    adminUser,
                    AppRoles.Admin);

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    roleResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Không thể gán role Admin: {errors}");
            }
        }
    }

    private static async Task SeedCategoriesAsync(
        ApplicationDbContext context)
    {
        var categories = new[]
        {
            new Category
            {
                Name = "CPU",
                Description = "Bộ vi xử lý máy tính",
                IsActive = true
            },
            new Category
            {
                Name = "GPU",
                Description = "Card đồ họa",
                IsActive = true
            },
            new Category
            {
                Name = "RAM",
                Description = "Bộ nhớ máy tính",
                IsActive = true
            },
            new Category
            {
                Name = "Laptop",
                Description = "Máy tính xách tay",
                IsActive = true
            }
        };

        foreach (var category in categories)
        {
            var exists = await context.Categories
                .AnyAsync(c => c.Name == category.Name);

            if (!exists)
            {
                await context.Categories.AddAsync(category);
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedProductsAsync(
        ApplicationDbContext context)
    {
        var categories = await context.Categories
            .ToDictionaryAsync(c => c.Name);

        var now = DateTime.UtcNow;

        var products = new[]
        {
            new Product
            {
                Name = "Intel Core i5-14600K",
                Price = 7990000m,
                StockQuantity = 15,
                Note = "Intel Core thế hệ 14",
                IsActive = true,
                IsFeatured = true,
                CreatedAt = now.AddDays(-16),
                CategoryId = categories["CPU"].Id
            },
            new Product
            {
                Name = "AMD Ryzen 7 7800X3D",
                Price = 10990000m,
                StockQuantity = 8,
                Note = "CPU AMD Ryzen 7",
                IsActive = true,
                IsFeatured = true,
                CreatedAt = now.AddDays(-15),
                CategoryId = categories["CPU"].Id
            },
            new Product
            {
                Name = "NVIDIA GeForce RTX 4070 SUPER",
                Price = 18990000m,
                StockQuantity = 6,
                Note = "Card đồ họa NVIDIA RTX 4070 SUPER",
                IsActive = true,
                IsFeatured = true,
                CreatedAt = now.AddDays(-14),
                CategoryId = categories["GPU"].Id
            },
            new Product
            {
                Name = "Kingston Fury Beast 32GB DDR5",
                Price = 2990000m,
                StockQuantity = 25,
                Note = "RAM DDR5 32GB",
                IsActive = true,
                IsFeatured = false,
                CreatedAt = now.AddDays(-13),
                CategoryId = categories["RAM"].Id
            },
            new Product
            {
                Name = "Corsair Vengeance 16GB DDR5",
                Price = 1690000m,
                StockQuantity = 30,
                Note = "RAM DDR5 16GB",
                IsActive = true,
                IsFeatured = false,
                CreatedAt = now.AddDays(-12),
                CategoryId = categories["RAM"].Id
            },
            new Product
            {
                Name = "ASUS TUF Gaming Laptop",
                Price = 24990000m,
                StockQuantity = 10,
                Note = "Laptop gaming ASUS TUF",
                IsActive = true,
                IsFeatured = true,
                CreatedAt = now.AddDays(-11),
                CategoryId = categories["Laptop"].Id
            },
            new Product
            {
                Name = "Lenovo ThinkPad",
                Price = 21990000m,
                StockQuantity = 7,
                Note = "Laptop văn phòng Lenovo",
                IsActive = true,
                IsFeatured = false,
                CreatedAt = now.AddDays(-10),
                CategoryId = categories["Laptop"].Id
            },
            new Product
            {
                Name = "Acer Nitro Gaming",
                Price = 22990000m,
                StockQuantity = 9,
                Note = "Laptop gaming Acer Nitro",
                IsActive = true,
                IsFeatured = true,
                CreatedAt = now.AddDays(-9),
                CategoryId = categories["Laptop"].Id
            },
            new Product
            {
                Name = "Intel Core i7-14700K",
                Price = 11290000m,
                StockQuantity = 11,
                Note = "Intel Core i7 thế hệ 14",
                IsActive = true,
                IsFeatured = true,
                CreatedAt = now.AddDays(-8),
                CategoryId = categories["CPU"].Id
            },
            new Product
            {
                Name = "AMD Ryzen 5 7600X",
                Price = 5990000m,
                StockQuantity = 18,
                Note = "CPU AMD Ryzen 5",
                IsActive = true,
                IsFeatured = false,
                CreatedAt = now.AddDays(-7),
                CategoryId = categories["CPU"].Id
            },
            new Product
            {
                Name = "NVIDIA GeForce RTX 4060",
                Price = 9990000m,
                StockQuantity = 14,
                Note = "Card đồ họa NVIDIA RTX 4060",
                IsActive = true,
                IsFeatured = false,
                CreatedAt = now.AddDays(-6),
                CategoryId = categories["GPU"].Id
            },
            new Product
            {
                Name = "AMD Radeon RX 7800 XT",
                Price = 15490000m,
                StockQuantity = 5,
                Note = "Card đồ họa AMD Radeon",
                IsActive = true,
                IsFeatured = true,
                CreatedAt = now.AddDays(-5),
                CategoryId = categories["GPU"].Id
            },
            new Product
            {
                Name = "G.Skill Trident Z5 32GB DDR5",
                Price = 3490000m,
                StockQuantity = 20,
                Note = "RAM DDR5 hiệu năng cao",
                IsActive = true,
                IsFeatured = false,
                CreatedAt = now.AddDays(-4),
                CategoryId = categories["RAM"].Id
            },
            new Product
            {
                Name = "Kingston Fury Beast 16GB DDR5",
                Price = 1590000m,
                StockQuantity = 35,
                Note = "RAM DDR5 16GB",
                IsActive = true,
                IsFeatured = false,
                CreatedAt = now.AddDays(-3),
                CategoryId = categories["RAM"].Id
            },
            new Product
            {
                Name = "Dell Inspiron 15",
                Price = 17990000m,
                StockQuantity = 12,
                Note = "Laptop Dell dành cho học tập và văn phòng",
                IsActive = true,
                IsFeatured = false,
                CreatedAt = now.AddDays(-2),
                CategoryId = categories["Laptop"].Id
            },
            new Product
            {
                Name = "HP Victus Gaming",
                Price = 23990000m,
                StockQuantity = 8,
                Note = "Laptop gaming HP Victus",
                IsActive = true,
                IsFeatured = true,
                CreatedAt = now.AddDays(-1),
                CategoryId = categories["Laptop"].Id
            }
        };

        foreach (var product in products)
        {
            var exists = await context.Products
                .AnyAsync(p => p.Name == product.Name);

            if (!exists)
            {
                await context.Products.AddAsync(product);
            }
        }

        await context.SaveChangesAsync();
    }
}