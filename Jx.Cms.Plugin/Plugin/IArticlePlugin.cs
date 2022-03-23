using System;
using System.Collections.Generic;
using BootstrapBlazor.Components;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.Plugin.Model;
using Microsoft.AspNetCore.Components;

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
        ArticleModel OnArticleShow(ArticleModel articleModel)
        {
            return articleModel;
        }

        /// <summary>
        /// 文章保存时处理
        /// </summary>
        /// <param name="articleEntity"></param>
        void OnArticleSaved(ArticleEntity articleEntity)
        {
        }

        /// <summary>
        /// 添加编辑器扩展按钮
        /// </summary>
        /// <returns></returns>
        EditorExtModel AddEditorToolbarButton(DialogService dialogService)
        {
            return null;
        }

        /// <summary>
        /// 下方扩展字段
        /// </summary>
        List<ArticleExtModel> BottomExt(ArticleEntity articleEntity)
        {
            return null;
        }

        /// <summary>
        /// 右侧扩展字段
        /// </summary>
        /// <param name="articleEntity"></param>
        /// <returns></returns>
        List<ArticleExtModel> RightExt(ArticleEntity articleEntity)
        {
            return null;
        }
    }
}