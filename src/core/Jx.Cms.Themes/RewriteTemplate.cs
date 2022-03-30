using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace Jx.Cms.Themes
{
    public static class RewriteTemplate
    {
        public static List<string> CreateUrl(string url)
        {
            var urlList = new List<string>();
            if (url == null)
            {
                return urlList;
            }
            var mc = Regex.Matches(url, @"\{\{(.+?)\}\}");
            foreach (Match match in mc)
            {
                urlList.Add(match.Groups[1].Value);
                if (match.Value == "{{year}}" || match.Value == "{{month}}" || match.Value == "{{day}}" || match.Value == "{{id}}"|| match.Value == "{{page}}")
                {
                    url = url.Replace(match.Value, "(\\d+?)");
                }
                else
                {
                    url = url.Replace(match.Value, "(.+?)");
                }
            }
            urlList.Insert(0, url);
            return urlList;
        }

        public static (bool isSuccess, Dictionary<string, string> result) AnalysisUrl(string baseUrl, List<string> urlList)
        {
            if (urlList.Count < 2)
            {
                return (false, null);
            }

            var mc = Regex.Match(baseUrl, urlList[0]);
            if (!mc.Success || mc.Groups.Count != urlList.Count)
            {
                return (false, null);
            }

            Dictionary<string, string> result = new Dictionary<string, string>();
            for (int i = 1; i < mc.Groups.Count; i++)
            {
                result.Add(urlList[i], HttpUtility.UrlDecode(mc.Groups[i].Value));
            }

            return (true, result);
        }
    }
}