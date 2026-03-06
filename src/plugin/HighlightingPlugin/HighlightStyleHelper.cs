using System.Reflection;
using BootstrapBlazor.Components;

namespace HighlightingPlugin;

internal static class HighlightStyleHelper
{
    private const string ResourcePrefix = "HighlightingPlugin.wwwroot.highlight.styles.";

    private static readonly Lazy<IReadOnlyList<string>> StyleFilesLazy = new(LoadStyleFiles);

    public const string DefaultStyleFile = "default.css";

    public static IReadOnlyList<string> GetStyleFiles()
    {
        return StyleFilesLazy.Value;
    }

    public static IEnumerable<SelectedItem> ToSelectItems()
    {
        return GetStyleFiles().Select(style => new SelectedItem(style, ToDisplayName(style)));
    }

    public static string Normalize(string styleFile)
    {
        if (string.IsNullOrWhiteSpace(styleFile)) return DefaultStyleFile;

        var fileName = Path.GetFileName(styleFile.Trim());
        if (!fileName.EndsWith(".css", StringComparison.OrdinalIgnoreCase)) fileName += ".css";

        var match = GetStyleFiles().FirstOrDefault(x => x.Equals(fileName, StringComparison.OrdinalIgnoreCase));
        return match ?? DefaultStyleFile;
    }

    private static IReadOnlyList<string> LoadStyleFiles()
    {
        var styles = Assembly.GetExecutingAssembly().GetManifestResourceNames()
            .Where(x => x.StartsWith(ResourcePrefix, StringComparison.Ordinal) &&
                        x.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
            .Select(x => x[ResourcePrefix.Length..])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x.Equals(DefaultStyleFile, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .ThenBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (styles.All(x => !x.Equals(DefaultStyleFile, StringComparison.OrdinalIgnoreCase)))
            styles.Insert(0, DefaultStyleFile);

        return styles;
    }

    private static string ToDisplayName(string styleFile)
    {
        if (string.IsNullOrWhiteSpace(styleFile)) return DefaultStyleFile;
        var name = Path.GetFileNameWithoutExtension(styleFile);
        var title = string.Join(" ", name.Split('-', StringSplitOptions.RemoveEmptyEntries));
        return $"{title} ({styleFile})";
    }
}
