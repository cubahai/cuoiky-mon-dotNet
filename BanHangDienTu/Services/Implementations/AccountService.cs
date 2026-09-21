using BanHangDienTu.Models.Constants;
using BanHangDienTu.Models.Entities;
using BanHangDienTu.Services.Interfaces;
using BanHangDienTu.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BanHangDienTu.Services.Implementations;

public sealed class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<(bool Succeeded, IReadOnlyList<string> Errors)>
        RegisterCustomerAsync(RegisterViewModel model)
    {
        var email = model.Email.Trim();

        var existingUser =
            await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            return (
                false,
                new[] { "Email này đã được sử dụng." });
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = model.FullName.Trim(),
            EmailConfirmed = false
        };

        var createResult =
            await _userManager.CreateAsync(
                user,
                model.Password);

        if (!createResult.Succeeded)
        {
            return (
                false,
                createResult.Errors
                    .Select(e => e.Description)
                    .ToList());
        }

        var roleResult =
            await _userManager.AddToRoleAsync(
                user,
                AppRoles.Customer);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            return (
                false,
                roleResult.Errors
                    .Select(e => e.Description)
                    .ToList());
        }

        return (
            true,
            Array.Empty<string>());
    }

    public async Task<(bool Succeeded, bool IsAdmin, string? ErrorMessage)>
        LoginAsync(LoginViewModel model)
    {
        var email = model.Email.Trim();

        var user =
            await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return (
                false,
                false,
                "Email hoặc mật khẩu không chính xác.");
        }

        var result =
            await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            return (
                false,
                false,
                "Tài khoản đang tạm khóa do đăng nhập sai quá nhiều lần.");
        }

        if (!result.Succeeded)
        {
            return (
                false,
                false,
                "Email hoặc mật khẩu không chính xác.");
        }

        var isAdmin =
            await _userManager.IsInRoleAsync(
                user,
                AppRoles.Admin);

        return (
            true,
            isAdmin,
            null);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<ProfileViewModel?> GetProfileAsync(
        string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        var user =
            await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return null;
        }

        return new ProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address
        };
    }

    public async Task<(bool Succeeded, IReadOnlyList<string> Errors)>
        UpdateProfileAsync(
            string userId,
            ProfileViewModel model)
    {
        var user =
            await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return (
                false,
                new[] { "Không tìm thấy tài khoản." });
        }

        user.FullName = model.FullName.Trim();

        user.PhoneNumber =
            string.IsNullOrWhiteSpace(model.PhoneNumber)
                ? null
                : model.PhoneNumber.Trim();

        user.Address =
            string.IsNullOrWhiteSpace(model.Address)
                ? null
                : model.Address.Trim();

        var result =
            await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return (
                false,
                result.Errors
                    .Select(e => e.Description)
                    .ToList());
        }

        await _signInManager.RefreshSignInAsync(user);

        return (
            true,
            Array.Empty<string>());
    }

    public async Task<(bool Succeeded, IReadOnlyList<string> Errors)>
        ChangePasswordAsync(
            string userId,
            ChangePasswordViewModel model)
    {
        var user =
            await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return (
                false,
                new[] { "Không tìm thấy tài khoản." });
        }

        var result =
            await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword);

        if (!result.Succeeded)
        {
            var errors = new List<string>();

            foreach (var error in result.Errors)
            {
                if (error.Code ==
                    nameof(IdentityErrorDescriber.PasswordMismatch))
                {
                    errors.Add(
                        "Mật khẩu hiện tại không chính xác.");
                }
                else
                {
                    errors.Add(error.Description);
                }
            }

            return (
                false,
                errors);
        }

        await _signInManager.RefreshSignInAsync(user);

        return (
            true,
            Array.Empty<string>());
    }
}