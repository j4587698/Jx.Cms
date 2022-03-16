using System.Collections.Generic;

namespace Jx.Cms.Plugin.Service.Front
{
    /// <summary>
    /// 分页相关
    /// </summary>
    public interface IPaginationService
    {
        /// <summary>
        /// 获取分页信息
        /// </summary>
        /// <param name="pageNo">当前页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="totalCount">总数量</param>
        /// <returns>key为</returns>
        Dictionary<string, int> GetPagination(int pageNo, int pageSize, int totalCount);
    }
}