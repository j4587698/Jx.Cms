using System.Collections.Generic;
using Jx.Cms.DbContext.Entities.Article;

namespace Jx.Cms.Plugin.Service.Both
{
    /// <summary>
    /// 标签相关服务
    /// </summary>
    public interface ITagService
    {
        /// <summary>
        /// 根据Id获取Label
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TagEntity GetLabelById(int id);
        
        /// <summary>
        /// 通过标签名查询标签是否已存在，并返回存在的标签
        /// </summary>
        /// <param name="labelNames">标签名列表</param>
        /// <returns>存在的标签列表</returns>
        List<TagEntity> LabelNameToLabels(List<string> labelNames);

        /// <summary>
        /// 将所有的标签名转换为标签，如果标签不存在，创建新类
        /// </summary>
        /// <param name="labelNames">标签名</param>
        /// <returns>所有标签列表</returns>
        List<TagEntity> AllLabelNameToLabels(List<string> labelNames);

        /// <summary>
        /// 获取指定标签ID下的所有文章
        /// </summary>
        /// <param name="id">标签Id</param>
        /// <param name="pageNumber">页码，从1开始</param>
        /// <param name="pageSize">每页多少条</param>
        /// <returns>文章列表</returns>
        List<ArticleEntity> GetArticleFormLabelId(int id, int pageNumber, int pageSize);
        
        /// <summary>
        /// 获取指定标签ID下的文章
        /// </summary>
        /// <param name="id">标签Id</param>
        /// <param name="pageNumber">页码，从1开始</param>
        /// <param name="pageSize">每页多少条</param>
        /// <param name="count">总条数</param>
        /// <returns>文章列表</returns>
        List<ArticleEntity> GetArticleFormLabelId(int id, int pageNumber, int pageSize, out long count);

        /// <summary>
        /// 获取指定标签名下所有文章（如果有多个同名标签，取第一个）
        /// </summary>
        /// <param name="name">标签名</param>
        /// <returns>文章列表</returns>
        List<ArticleEntity> GetArticleFormLabelName(string name);
    }
}