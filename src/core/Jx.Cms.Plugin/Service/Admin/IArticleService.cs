﻿using System.Collections.Generic;
using Jx.Cms.DbContext.Entities.Article;

namespace Jx.Cms.Plugin.Service.Admin
{
    /// <summary>
    /// 文章服务
    /// </summary>
    public interface IArticleService
    {
        /// <summary>
        /// 根据ID获取文章
        /// </summary>
        /// <param name="id">文章ID</param>
        /// <returns>对应的文章</returns>
        ArticleEntity GetArticleById(int id);

        /// <summary>
        /// 获取所有文章
        /// </summary>
        /// <returns>所有文章列表</returns>
        List<ArticleEntity> GetAllArticle();

        /// <summary>
        /// 分页获取文章
        /// </summary>
        /// <param name="pageNumber">获取第几页</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="count">文章总数量</param>
        /// <returns>指定页文章列表</returns>
        List<ArticleEntity> GetArticlePage(int pageNumber, int pageSize, out long count);

        /// <summary>
        /// 分页获取文章
        /// </summary>
        /// <param name="pageNumber">获取第几页</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>制定页文章列表</returns>
        List<ArticleEntity> GetArticlePage(int pageNumber, int pageSize);
        
        /// <summary>
        /// 根据标签名获取文章
        /// </summary>
        /// <param name="label">标签名（如果标签名相同则会全部查出）</param>
        /// <param name="pageNumber">获取第几页</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="count">文章总数量</param>
        /// <returns>指定页文章列表</returns>
        List<ArticleEntity> GetArticleByLabel(string label, int pageNumber, int pageSize, out long count);

        /// <summary>
        /// 保存文章
        /// </summary>
        /// <param name="articleEntity">文章</param>
        /// <returns>是否成功</returns>
        bool SaveArticle(ArticleEntity articleEntity);

        /// <summary>
        /// 删除文章，同时删除meta和tag
        /// </summary>
        /// <param name="articleEntity"></param>
        /// <returns></returns>
        bool DeleteArticle(ArticleEntity articleEntity);
    }
}