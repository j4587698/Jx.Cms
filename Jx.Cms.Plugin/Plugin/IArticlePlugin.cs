using System;
using Jx.Cms.Plugin.Model;

namespace Jx.Cms.Plugin.Plugin
{
    /// <summary>
    /// 文章插件接口
    /// </summary>
    public interface IArticlePlugin
    {
        /// <summary>
        /// 文章展示前处理
        /// </summary>
        ArticleModel OnArticleShow(ArticleModel articleModel);
    }
}