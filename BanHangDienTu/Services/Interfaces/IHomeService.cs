using BanHangDienTu.ViewModels.Home;
using System.Threading.Tasks;

namespace BanHangDienTu.Services.Interfaces;

public interface IHomeService
{
    Task<HomeViewModel> GetHomeDataAsync(string? searchTerm = null);
}