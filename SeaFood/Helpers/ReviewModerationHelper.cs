namespace SeaFood.Helpers;

public static class ReviewModerationHelper
{
    private const string PendingPrefix = "[PENDING]";

    public static string ToPending(string comment) => $"{PendingPrefix}{comment}";
    public static bool IsPending(string comment) => comment.StartsWith(PendingPrefix, StringComparison.Ordinal);
    public static string CleanForDisplay(string comment) => IsPending(comment) ? comment[PendingPrefix.Length..] : comment;
    public static string Approve(string comment) => CleanForDisplay(comment);
}
