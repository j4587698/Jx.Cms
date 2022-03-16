using System.Collections.Generic;
using Furion.DependencyInjection;

namespace Jx.Cms.Plugin.Service.Front.Impl
{
    public class PaginationService: IPaginationService, ITransient
    {
        public Dictionary<string, int> GetPagination(int pageNo, int pageSize, int totalCount)
        {
            if (pageNo == 0)
            {
                pageNo = 1;
            }
            var result = new Dictionary<string, int>();
            var totalPage = totalCount % pageSize == 0 ? totalCount / pageSize : totalCount / pageSize + 1;
            var startNo = 1;
            if (totalPage <= 10)
            {
                startNo = 1;
            }
            else if (pageNo + 5 > totalPage)
            {
                startNo = pageNo - 4;
            }
            else if(pageNo - 4 < 1)
            {
                startNo = 1;
            }
            else
            {
                startNo = pageNo - 4;
            }

            if (pageNo != 1)
            {
                result.Add("<<", 1);
            }
            if (pageNo > 1)
            {
                result.Add("<", pageNo - 1);
            }
            for (int i = startNo; i <= pageNo + 5 && i <= totalPage; i++)
            {
                result.Add(i.ToString(), i);
            }
            if (pageNo < totalPage)
            {
                result.Add(">", pageNo + 1);
            }
            if (pageNo != totalPage)
            {
                result.Add(">>", totalPage);
            }
            return result;
        }
    }
}