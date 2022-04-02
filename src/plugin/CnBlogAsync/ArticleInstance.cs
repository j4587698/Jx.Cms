using CnBlogAsync.Models;
using CnBlogAsync.Options;
using Furion;
using Jx.Cms.Common.Enum;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Plugin;
using Jx.Cms.Plugin.Service.Both;
using Jx.Cms.Plugin.Service.Front;
using Markdig;
using Masuit.Tools;
using Masuit.Tools.AspNetCore.Mime;
using Masuit.Tools.Html;
using StackExchange.Profiling.Internal;

namespace CnBlogAsync;

public class ArticleInstance : IArticlePlugin
{
    public List<ArticleExtModel> RightExt(ArticleEntity articleEntity)
    {
        var defaultValue = App.GetService<ISettingsService>().GetValue(Constants.PluginName, Constants.BlogDefaultValue);
        if (defaultValue.IsNullOrEmpty())
        {
            return new List<ArticleExtModel>();
        }
        var models = new List<ArticleExtModel>
        {
            new ArticleExtModel()
            {
                ArticleExtTypeEnum = ArticleExtTypeEnum.Select,
                DefaultValue = defaultValue,
                DisplayName = "是否同步文章到博客园",
                Items = new List<string>()
                {
                    "是", "否"
                },
                Name = Constants.EnableFlag,
                PluginName = Constants.PluginName
            }
        };
        return models;
    }

    public bool OnArticleBeforeSave(ArticleEntity articleEntity, out string errorMsg)
    {
        errorMsg = "";
        if (articleEntity.Metas.First(x => x.PluginName == Constants.PluginName && x.Name == Constants.EnableFlag).Value != "是")
        {
            return true;
        }

        var values = App.GetService<ISettingsService>().GetAllValues(Constants.PluginName);
        if (!values.ContainsKey(Constants.BlogName))
        {
            return true;
        }
        var option = new CnBlogsOption(values[Constants.BlogName], values[Constants.BlogUserName],
            values[Constants.BlogPassword]);
        var client = new Client(option);
        client.GetUsersBlogs();
        var content = articleEntity.Content;
        if (articleEntity.IsMarkdown)
        {
            content = Markdown.ToHtml(articleEntity.Content);
        }

        var imgs = content.MatchImgSrcs();
        var mimeMapper = new MimeMapper();
        foreach (var img in imgs)
        {
            if (!img.StartsWith("\\") && !img.StartsWith("/"))
            {
                continue;
            }

            var bytes = File.ReadAllBytes(Path.Combine(App.WebHostEnvironment.WebRootPath, img.TrimStart('\\', '/')));
            var media = client.NewMediaObject(Path.GetFileName(img),
                mimeMapper.GetMimeFromExtension(Path.GetExtension(img)), bytes);
            content = content.Replace(img, media.URL);
        }

        var categories = client.GetCategories();
        string categoryName = null;
        var catalogue = App.GetService<ICatalogService>().GetCatalogById(articleEntity.CatalogueId);
        if (catalogue != null)
        {
            if (!categories.Any(x => x.Title == $"[随笔分类]{catalogue.Name}"))
            {
                client.NewCategory(new WpCategory()
                {
                    Description = catalogue.Description,
                    Name = catalogue.Name,
                    ParentId = 0
                });
            }
            categoryName = $"[随笔分类]{catalogue.Name}";
        }

        if (articleEntity.Metas.Any(x => x.PluginName == Constants.PluginName && x.Name == Constants.BlogId))
        {
            client.EditPost(
                articleEntity.Metas.First(x => x.PluginName == Constants.PluginName && x.Name == Constants.BlogId).Value,
                articleEntity.Title, content, new List<string>(){categoryName}, true);
        }
        else
        {
            var postId = client.NewPost(articleEntity.Title, content, new List<string>(){categoryName},
                DateTime.Now);
            articleEntity.Metas.Add(new ArticleMetaEntity()
            {
                ArticleId = articleEntity.Id,
                Name = Constants.BlogId,
                PluginName = Constants.PluginName,
                Value = postId
            });
        }

        return true;
    }
}