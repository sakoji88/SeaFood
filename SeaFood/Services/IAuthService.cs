using SeaFood.Models;
using SeaFood.ViewModels;

namespace SeaFood.Services;

public interface IAuthService
{
    Task<(bool Ok, string Error)> RegisterAsync(RegisterViewModel model);
    Task<(bool Ok, string Error, User? User)> ValidateLoginAsync(LoginViewModel model);
}
