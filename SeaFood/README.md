# Kacer Fish (ASP.NET Core MVC, .NET 8)

Учебный проект интернет-магазина морепродуктов с авторизацией на Cookie Authentication без ASP.NET Identity.

## Стек
- ASP.NET Core MVC (.NET 8)
- EF Core + SQL Server LocalDB
- Bootstrap 5 + кастомный CSS

## Подключение к БД
Строка подключения уже добавлена в `appsettings.json`:

`Server=(localdb)\\mssqllocaldb;Database=SeafoodStore;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True`

## Запуск
```bash
dotnet restore
dotnet build
dotnet run
```

## Если БД уже существует
Проект ориентирован на существующую схему `SeafoodStore`.

## Если нужно пересобрать сущности из БД (опционально)
```bash
dotnet tool install --global dotnet-ef
dotnet ef dbcontext scaffold "Server=(localdb)\\mssqllocaldb;Database=SeafoodStore;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -o Models -c AppDbContext
```
