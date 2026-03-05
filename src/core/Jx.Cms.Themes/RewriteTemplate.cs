using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Jx.Cms.Themes;

public static class RewriteTemplate
{
    public static List<string> CreateUrl(string url)
    {
        var urlList = new List<string>();
        if (string.IsNullOrWhiteSpace(url)) return urlList;

        var matches = Regex.Matches(url, @"\{\{([a-zA-Z0-9_]+)\}\}");
        var regex = new StringBuilder("^");
        var startIndex = 0;

        foreach (Match match in matches)
        {
            if (!match.Success) continue;

            regex.Append(Regex.Escape(url[startIndex..match.Index]));
            var key = match.Groups[1].Value;
            urlList.Add(key);
            regex.Append(GetTokenRegex(key));
            startIndex = match.Index + match.Length;
        }

        regex.Append(Regex.Escape(url[startIndex..]));
        regex.Append('$');

        urlList.Insert(0, regex.ToString());
        return urlList;
    }

    public static (bool isSuccess, Dictionary<string, string> result) AnalysisUrl(string baseUrl, List<string> urlList)
    {
        if (string.IsNullOrWhiteSpace(baseUrl) || urlList == null || urlList.Count < 2) return (false, null);

        var mc = Regex.Match(baseUrl, urlList[0], RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (!mc.Success || mc.Groups.Count != urlList.Count) return (false, null);

        var result = new Dictionary<string, string>();
        for (var i = 1; i < mc.Groups.Count; i++) result[urlList[i]] = HttpUtility.UrlDecode(mc.Groups[i].Value);

        return (true, result);
    }

    private static string GetTokenRegex(string key)
    {
        return key switch
        {
            "year" or "month" or "day" or "id" or "page" => "(\\d+)",
            _ => "([^/]+)"
        };
    }
}
