using System.Collections.Generic;
using BootstrapBlazor.Components;
using Jx.Cms.Common.Components;
using Jx.Cms.Common.Enum;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Plugin;
using Microsoft.AspNetCore.Components;

namespace Jx.Cms.Admin;

public class TestPlugin : IArticlePlugin
{
    private TestComponet _testComponent;
    
    List<ArticleExtModel> _articleExtModels = new List<ArticleExtModel>()
    {
        new ArticleExtModel()
        {
            ArticleExtTypeEnum = ArticleExtTypeEnum.Input,
            DisplayName = "test",
            Name = "test",
            DefaultValue = "ccc"
        }
    };

    public bool OnArticleBeforeSave(ArticleEntity articleEntity)
    {
        return true;
    }

    public List<ArticleExtModel> RightExt(ArticleEntity articleEntity)
    {
        return _articleExtModels;
    }
}