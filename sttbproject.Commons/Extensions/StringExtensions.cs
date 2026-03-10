using System.Text;
using System.Text.RegularExpressions;

namespace sttbproject.Commons.Extensions;

public static class StringExtensions
{
    public static string ToSlug(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        text = text.ToLowerInvariant();
        text = Regex.Replace(text, @"\s+", "-");
        text = Regex.Replace(text, @"[^a-z0-9\-]", "");
        text = Regex.Replace(text, @"-+", "-");
        text = text.Trim('-');

        return text;
    }

    public static string Truncate(this string text, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
            return text;

        return text.Substring(0, maxLength - suffix.Length) + suffix;
    }
}
