using CnBlogAsync.Models;
using CnBlogAsync.Options;
using Jx.Cms.Common.Enum;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Plugin;
using Jx.Cms.Plugin.Service.Both;
using Jx.Cms.Plugin.Service.Front;
using Jx.Toolbox.Extensions;
using Jx.Toolbox.HtmlTools;
using Jx.Toolbox.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CnBlogAsync;

public class ArticleInstance : IArticlePlugin
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ArticleInstance(IServiceProvider serviceProvider, IWebHostEnvironment webHostEnvironment)
    {
        _serviceProvider = serviceProvider;
        _webHostEnvironment = webHostEnvironment;
    }

    public List<ArticleExtModel> RightExt(ArticleEntity articleEntity)
    {
        var settingsService = _serviceProvider.GetRequiredService<ISettingsService>();
        var defaultValue = settingsService.GetValue(Constants.PluginName, Constants.BlogDefaultValue);
        if (defaultValue.IsNullOrEmpty()) return new List<ArticleExtModel>();
        var models = new List<ArticleExtModel>
        {
            new()
            {
                ArticleExtTypeEnum = ArticleExtTypeEnum.Select,
                DefaultValue = defaultValue,
                DisplayName = "是否同步文章到博客园",
                Items = new List<string>
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
        if (articleEntity.Status != ArticleStatusEnum.Published) return true;
        articleEntity.Metas ??= new List<ArticleMetaEntity>();
        var enableMeta = articleEntity.Metas.FirstOrDefault(x =>
            x.PluginName == Constants.PluginName && x.Name == Constants.EnableFlag);
        if (enableMeta == null || enableMeta.Value != "是") return true;

        var settingsService = _serviceProvider.GetRequiredService<ISettingsService>();
        var values = settingsService.GetAllValues(Constants.PluginName);
        if (!values.ContainsKey(Constants.BlogName)) return true;
        var option = new CnBlogsOption(values[Constants.BlogName], values[Constants.BlogUserName],
            values[Constants.BlogPassword]);
        var client = new Client(option);
        client.GetUsersBlogs();
        var content = articleEntity.Content;

        var imgs = Html.GetAllImgSrc(content);
        foreach (var img in imgs)
        {
            if (!img.StartsWith("\\") && !img.StartsWith("/")) continue;

            var bytes = File.ReadAllBytes(Path.Combine(_webHostEnvironment.WebRootPath, img.TrimStart('\\', '/')));
            var media = client.NewMediaObject(Path.GetFileName(img),
                Mime.GetMimeFromExtension(Path.GetExtension(img)), bytes);
            content = content.Replace(img, media.URL);
        }

        var categories = client.GetCategories();
        string categoryName = null;
        var catalogService = _serviceProvider.GetRequiredService<ICatalogService>();
        var catalogue = catalogService.GetCatalogById(articleEntity.CatalogueId);
        if (catalogue != null)
        {
            if (categories.All(x => x.Title != $"[随笔分类]{catalogue.Name}"))
                client.NewCategory(new WpCategory
                {
                    Description = catalogue.Description,
                    Name = catalogue.Name,
                    ParentId = 0
                });
            categoryName = $"[随笔分类]{catalogue.Name}";
        }

        var blogIdMeta = articleEntity.Metas.FirstOrDefault(x =>
            x.PluginName == Constants.PluginName && x.Name == Constants.BlogId);
        if (blogIdMeta != null)
        {
            client.EditPost(blogIdMeta.Value, articleEntity.Title, content, new List<string> { categoryName }, true);
        }
        else
        {
            var postId = client.NewPost(articleEntity.Title, content, new List<string> { categoryName },
                DateTime.Now);
            articleEntity.Metas.Add(new ArticleMetaEntity
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
