using System.Collections.Generic;
using System.Linq;
using Furion.DependencyInjection;
using Jx.Cms.DbContext.Entities.Article;

namespace Jx.Cms.Plugin.Service.Front.Impl;

public class CatalogService : ICatalogService, ITransient
{
    public CatalogEntity GetCatalogById(int id)
    {
        return CatalogEntity.Find(id);
    }

    public List<ArticleEntity> GetArticlesByCatalogId(int id, bool includeChildren, int pageNumber, int pageSize, out long count)
    {
        List<int> catalogues = new List<int>();
        if (includeChildren)
        {
            catalogues = CatalogEntity.Where(x => x.Id == id).AsTreeCte().ToList(x => x.Id);
        }
        else
        {
            catalogues.Add(id);
        }

        return ArticleEntity.Where(x => x.IsPage == false && catalogues.Contains(x.CatalogueId))
            .OrderByDescending(x => x.PublishTime).Page(pageNumber, pageSize).Count(out count)
            .IncludeMany(x => x.Comments.Select(y => new CommentEntity() { Id = y.Id }))
            .Include(x => x.Catalogue).ToList();
    }
    
}