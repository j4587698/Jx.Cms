using System.Collections.Generic;
using Furion.DependencyInjection;
using Jx.Cms.Entities.Article;

namespace Jx.Cms.Service.Impl
{
    public class CatalogService: ICatalogService, ITransient
    {
        public CatalogEntity FindCatalogById(int id)
        {
            return CatalogEntity.Find(id) ?? new CatalogEntity();
        }

        public List<CatalogEntity> GetCatalogPageWithCount(int pageNumber, int pageSize, out long count)
        {
            return CatalogEntity.Select.Count(out count).Page(pageNumber, pageSize).ToList();
        }

        public bool Save(CatalogEntity catalogEntity)
        {
            catalogEntity.Save();
            return true;
        }
    }
}