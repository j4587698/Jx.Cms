using System;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Plugin;

namespace HighlightingPlugin
{
    public class ArticleInstance : IArticlePlugin
    {
        public ArticleModel OnArticleShow(ArticleModel articleModel)
        {
            articleModel.Body += "Highlighting Run";
            return articleModel;
        }
    }
}