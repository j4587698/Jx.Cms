using System;
using System.IO;
using System.Text.Encodings.Web;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.Plugin.Plugin;
using Jx.Cms.Themes.Util;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Jx.Cms.Admin.Widgets;

public class ArchiveWidget : IWidget
{
    public string Name { get; set; } = "Archive";
    public string DisplayName { get; set; } = "文章归档";
    public string Description { get; set; } = "按年月对文章进行分类归档";
    public Type SystemBodyType { get; set; }
    public string Parameter { get; set; }
    public string GetWidgetName()
    {
        return "文章归档";
    }

    public string GetWidgetHtml()
    {
        var list = ArticleEntity.Select.Where(x => !x.IsPage)
            .GroupBy(x => x.PublishTime.ToString("yyyy年MM月")).OrderByDescending(x => x.Value.PublishTime).Take(20)
            .ToList(x => new { x.Key, count = x.Count(), publishTime = x.Value.PublishTime });
        TagBuilder ulTag = new TagBuilder("ul");
        foreach (var archive in list)
        {
            var liTag = new TagBuilder("li");
            var aTag = new TagBuilder("a");
            aTag.MergeAttribute("href", RewriteUtil.GetDateUrl(archive.publishTime.Year, archive.publishTime.Month, 1));
            aTag.MergeAttribute("title", archive.Key);
            aTag.InnerHtml.Append($"{archive.Key}({archive.count})");
            liTag.InnerHtml.AppendHtml(aTag);
            ulTag.InnerHtml.AppendHtml(liTag);
        }

        using TextWriter writer = new StringWriter();
        ulTag.WriteTo(writer, HtmlEncoder.Default);
        return writer.ToString();
    }
}