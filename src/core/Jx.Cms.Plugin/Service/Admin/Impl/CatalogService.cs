using System.Collections.Generic;
using FreeSql;
using Furion.DependencyInjection;
using Furion.Logging.Extensions;
using Jx.Cms.Common.Exceptions;
using Jx.Cms.DbContext.Entities.Article;

namespace Jx.Cms.Plugin.Service.Admin.Impl
{
    public class CatalogService: ICatalogService, ITransient
    {
        public CatalogEntity FindCatalogById(int id)
        {
            return CatalogEntity.Find(id) ?? new CatalogEntity();
        }

        public List<CatalogEntity> GetAllCatalogs()
        {
            return CatalogEntity.Select.OrderByDescending(x => x.Id).ToList();
        }

        public List<CatalogEntity> GetCatalogPage(int pageNumber, int pageSize, out long count)
        {
            return CatalogEntity.Select.Count(out count).OrderByDescending(x => x.Id).Page(pageNumber, pageSize).ToList();
        }

        public List<CatalogEntity> GetCatalogPage(int pageNumber, int pageSize)
        {
            return CatalogEntity.Select.OrderByDescending(x => x.Id).Page(pageNumber, pageSize).ToList();
        }

        public bool Save(CatalogEntity catalogEntity)
        {
            catalogEntity.Save();
            return true;
        }

        public bool Delete(IEnumerable<CatalogEntity> catalogEntities)
        {
            try
            {
                BaseEntity.Orm.Transaction(() =>
                {
                    foreach (var catalogEntity in catalogEntities)
                    {
                        if (!catalogEntity.Delete(true))
                        {
                            throw new DbException("删除失败");
                        }
                    }
                });
            }
            catch (DbException e)
            {
                "删除失败".LogError<CatalogService>(e);
                return false;
            }
            
            return true;
        }
    }
}