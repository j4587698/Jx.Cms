using FreeSql;
using Jx.Cms.Common.Exceptions;
using Jx.Cms.DbContext.Entities.Article;
using Serilog;

namespace Jx.Cms.Plugin.Service.Admin.Impl;

public class CatalogService : ICatalogService
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
        return CatalogEntity.Select
            .Include(x => x.Parent)
            .Count(out count)
            .OrderByDescending(x => x.Id)
            .Page(pageNumber, pageSize)
            .ToList();
    }

    public List<CatalogEntity> GetCatalogPage(int pageNumber, int pageSize)
    {
        return CatalogEntity.Select
            .Include(x => x.Parent)
            .OrderByDescending(x => x.Id)
            .Page(pageNumber, pageSize)
            .ToList();
    }

    public bool Save(CatalogEntity catalogEntity)
    {
        if (catalogEntity == null) return false;

        if (catalogEntity.ParentId < 0) catalogEntity.ParentId = 0;
        if (catalogEntity.Id > 0 && catalogEntity.ParentId == catalogEntity.Id) return false;

        if (catalogEntity.ParentId > 0 && CatalogEntity.Find(catalogEntity.ParentId) == null)
        {
            catalogEntity.ParentId = 0;
        }

        // 防止目录关系形成环（例如 A -> B，B -> A）。
        if (catalogEntity.Id > 0 && catalogEntity.ParentId > 0)
        {
            var visited = new HashSet<int> { catalogEntity.Id };
            var currentParentId = catalogEntity.ParentId;
            while (currentParentId > 0)
            {
                if (!visited.Add(currentParentId)) return false;

                var parent = CatalogEntity.Find(currentParentId);
                if (parent == null) break;
                currentParentId = parent.ParentId;
            }
        }

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
                    if (!catalogEntity.Delete(true))
                        throw new DbException("删除失败");
            });
        }
        catch (DbException e)
        {
            Log.Error(e, "删除失败");
            return false;
        }

        return true;
    }
}
