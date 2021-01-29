using System.Collections.Generic;
using Jx.Cms.Entities.Article;

namespace Jx.Cms.Service.Admin
{
    /// <summary>
    /// 标签相关服务
    /// </summary>
    public interface ILabelService
    {
        /// <summary>
        /// 通过标签名查询标签是否已存在，并返回存在的标签
        /// </summary>
        /// <param name="labelNames">标签名列表</param>
        /// <returns>存在的标签列表</returns>
        List<LabelEntity> LabelNameToLabels(List<string> labelNames);

        /// <summary>
        /// 将所有的标签名转换为标签，如果标签不存在，创建新类
        /// </summary>
        /// <param name="labelNames">标签名</param>
        /// <returns>所有标签列表</returns>
        List<LabelEntity> AllLabelNameToLabels(List<string> labelNames);
    }
}