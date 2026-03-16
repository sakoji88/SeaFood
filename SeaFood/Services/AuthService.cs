using Microsoft.EntityFrameworkCore;
using SeaFood.Data;
using SeaFood.Helpers;
using SeaFood.Models;
using SeaFood.ViewModels;

namespace SeaFood.Services;

// Сервис регистрации и входа пользователя без ASP.NET Identity.
public class AuthService(AppDbContext db, IPasswordHasher hasher) : IAuthService
{
    public async Task<(bool Ok, string Error)> RegisterAsync(RegisterViewModel model)
    {
        var fullName = StringSanitizer.Clean(model.FullName, 150);
        var email = StringSanitizer.Clean(model.Email, 150).ToLowerInvariant();
        var phone = StringSanitizer.Clean(model.Phone, 30);

        if (StringSanitizer.IsBlank(fullName) || StringSanitizer.IsBlank(email) || StringSanitizer.IsBlank(phone))
        {
            return (false, "Поля не должны состоять только из пробелов");
        }

        var emailExists = await db.Users.AnyAsync(x => x.Email == email);
        if (emailExists)
        {
            return (false, "Пользователь с таким email уже существует");
        }

        var userRole = await db.Roles.FirstOrDefaultAsync(x => x.Name == "User");
        if (userRole is null)
        {
            return (false, "В базе данных отсутствует роль User");
        }

        var user = new User
        {
            FullName = fullName,
            Email = email,
            Phone = phone,
            PasswordHash = hasher.HashPassword(model.Password),
            RoleId = userRole.Id,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
        return (true, string.Empty);
    }

    public async Task<(bool Ok, string Error, User? User)> ValidateLoginAsync(LoginViewModel model)
    {
        var email = StringSanitizer.Clean(model.Email, 150).ToLowerInvariant();
        var user = await db.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Email == email);
        if (user is null)
        {
            return (false, "Пользователь с таким email не найден", null);
        }

        if (!user.IsActive)
        {
            return (false, "Ваш аккаунт заблокирован", null);
        }

        if (!hasher.VerifyPassword(model.Password, user.PasswordHash))
        {
            return (false, "Неверный пароль", null);
        }

        return (true, string.Empty, user);
    }
}
