using BanHangDienTu.Data;
using BanHangDienTu.Models.Entities;
using BanHangDienTu.Repositories.Implementations;
using BanHangDienTu.Repositories.Interfaces;
using BanHangDienTu.Services.Identity;
using BanHangDienTu.Services.Implementations;
using BanHangDienTu.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BanHangDienTu;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder =
            WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<ApplicationDbContext>(
            options =>
                options.UseSqlServer(
                    builder.Configuration
                        .GetConnectionString(
                            "DefaultConnection")));

        builder.Services
            .AddIdentity<
                ApplicationUser,
                IdentityRole>(
                options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = false;

                    options.User.RequireUniqueEmail = true;

                    options.Lockout.MaxFailedAccessAttempts = 5;

                    options.Lockout.DefaultLockoutTimeSpan =
                        TimeSpan.FromMinutes(5);
                })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddScoped<
            IUserClaimsPrincipalFactory<ApplicationUser>,
            ApplicationUserClaimsPrincipalFactory>();

        builder.Services.ConfigureApplicationCookie(
            options =>
            {
                options.LoginPath =
                    "/Account/Login";

                options.AccessDeniedPath =
                    "/Account/AccessDenied";

                options.Cookie.HttpOnly = true;

                options.SlidingExpiration = true;

                options.ExpireTimeSpan =
                    TimeSpan.FromHours(2);
            });

        builder.Services.AddScoped<
            IProductRepository,
            ProductRepository>();

        builder.Services.AddScoped<
            ICategoryRepository,
            CategoryRepository>();

        builder.Services.AddScoped<
            IHomeService,
            HomeService>();

        builder.Services.AddScoped<
            IProductService,
            ProductService>();

        builder.Services.AddScoped<
            IAccountService,
            AccountService>();

        builder.Services.AddScoped<ICartRepository, CartRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<ICartService, CartService>();
        builder.Services.AddScoped<IOrderService, OrderService>();

        var app =
            builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler(
                "/Home/Error");

            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "areas",
            pattern:
                "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

        app.MapControllerRoute(
                name: "default",
                pattern:
                    "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        if (app.Environment.IsDevelopment())
        {
            await DbInitializer.SeedAsync(
                app.Services);
        }

        await app.RunAsync();
    }
}
