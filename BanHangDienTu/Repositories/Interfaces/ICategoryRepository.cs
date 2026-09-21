using BanHangDienTu.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BanHangDienTu.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetActiveCategoriesAsync();
}