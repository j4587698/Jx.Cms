using System.Text.RegularExpressions;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Toolbox.Extensions;
using Microsoft.AspNetCore.Http;

namespace PulseBlog.Utils;

public static class ThemeViewHelper
{
    private static readonly Regex ImgRegex = new(
        "<img\\b[^<>]*?\\bsrc[\\s\\t\\r\\n]*=[\\s\\t\\r\\n]*[\"']?[\\s\\t\\r\\n]*(?<imgUrl>[^\\s\\t\\r\\n\"'<>]*)[^<>]*?/?[\\s\\t\\r\\n]*>",
        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

    private static readonly Regex ColorRegex = new("^#?[0-9a-fA-F]{6}$", RegexOptions.Compiled);

    public static string NormalizeColor(string color)
    {
        if (color.IsNullOrEmpty()) return "#0A7F78";
        var trimmed = color.Trim();
        if (!ColorRegex.IsMatch(trimmed)) return "#0A7F78";
        return trimmed.StartsWith("#") ? trimmed : $"#{trimmed}";
    }

    public static string ToAbsoluteUrl(this HttpContext httpContext, string url)
    {
        if (url.IsNullOrEmpty()) return "";
        if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return url;
        return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{(url.StartsWith("/") ? "" : "/")}{url}";
    }

    public static string GetPlainText(string html, int maxLength = 160)
    {
        if (html.IsNullOrEmpty()) return "";
        var text = Jx.Toolbox.HtmlTools.Html.RemoveHtmlTag(html).Trim();
        if (text.Length <= maxLength) return text;
        return text[..maxLength];
    }

    public static string GetFirstImage(string html)
    {
        if (html.IsNullOrEmpty()) return "";
        var match = ImgRegex.Match(html);
        if (!match.Success) return "";
        return match.Groups["imgUrl"].Value;
    }

    public static string GetCover(this ArticleEntity article, string fallbackImage)
    {
        var image = GetFirstImage(article.Content);
        return image.IsNullOrEmpty() ? fallbackImage : image;
    }
}
