using BanHangDienTu.ViewModels.Account;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BanHangDienTu.Services.Interfaces;

public interface IAccountService
{
    Task<(bool Succeeded, IReadOnlyList<string> Errors)>
        RegisterCustomerAsync(RegisterViewModel model);

    Task<(bool Succeeded, bool IsAdmin, string? ErrorMessage)>
        LoginAsync(LoginViewModel model);

    Task LogoutAsync();

    Task<ProfileViewModel?> GetProfileAsync(string userId);

    Task<(bool Succeeded, IReadOnlyList<string> Errors)>
        UpdateProfileAsync(
            string userId,
            ProfileViewModel model);

    Task<(bool Succeeded, IReadOnlyList<string> Errors)>
        ChangePasswordAsync(
            string userId,
            ChangePasswordViewModel model);
}