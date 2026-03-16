namespace SeaFood.Helpers;

public static class StringSanitizer
{
    public static string Clean(string? value, int maxLength)
    {
        var trimmed = (value ?? string.Empty).Trim();
        return trimmed.Length > maxLength ? trimmed[..maxLength] : trimmed;
    }

    public static bool IsBlank(string? value) => string.IsNullOrWhiteSpace(value);
}
