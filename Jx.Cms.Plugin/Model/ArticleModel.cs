using Jx.Cms.DbContext.Entities.Article;

namespace Jx.Cms.Plugin.Model
{
    /// <summary>
    /// 文章插件传送内容
    /// </summary>
    public class ArticleModel
    {
        /// <summary>
        /// 文章主体内容
        /// </summary>
        public ArticleEntity Body { get; set; }

        /// <summary>
        /// 评论总数
        /// </summary>
        public long CommentCount { get; set; }

        /// <summary>
        /// 头部添加内容(如css)
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// 底部添加内容(如js)
        /// </summary>
        public string Footer { get; set; }
    }
}