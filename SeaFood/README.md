# Kacer Fish (ASP.NET Core MVC, .NET 8)

Учебный проект интернет-магазина морепродуктов с авторизацией на Cookie Authentication без ASP.NET Identity.

## Стек
- ASP.NET Core MVC (.NET 8)
- EF Core + SQL Server LocalDB
- Bootstrap 5 + кастомный CSS

## Подключение к БД
Строка подключения уже добавлена в `appsettings.json`:

`Server=(localdb)\\mssqllocaldb;Database=SeafoodStore;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True`

## Демо-учетная запись администратора
При запуске приложение устанавливает пароль `Admin123!` для `admin@seafoodstore.ru` (если этот пользователь уже есть в БД и имеет роль `Admin`).

## Демо-корзина и отзывы
- Авторизованный пользователь добавляет товар в корзину в кг.
- При демо-оплате остаток `StockKg` уменьшается в БД.
- После покупки пользователь может оставить отзыв на купленный товар.
- Отзыв отправляется на модерацию; администратор одобряет его в разделе `Админка -> Отзывы`.

## Запуск
```bash
dotnet restore
dotnet build
dotnet run
```

## Если нужно пересобрать сущности из БД (опционально)
```bash
dotnet tool install --global dotnet-ef
dotnet ef dbcontext scaffold "Server=(localdb)\\mssqllocaldb;Database=SeafoodStore;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -o Models -c AppDbContext
```
