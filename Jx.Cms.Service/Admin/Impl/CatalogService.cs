﻿using System.Collections.Generic;
using FreeSql;
using Furion.DependencyInjection;
using Furion.Logging.Extensions;
using Jx.Cms.Common.Exceptions;
using Jx.Cms.Entities.Article;
using Microsoft.Extensions.Logging;

namespace Jx.Cms.Service.Admin.Impl
{
    public class CatalogService: ICatalogService, ITransient
    {
        public CatalogEntity FindCatalogById(int id)
        {
            return CatalogEntity.Find(id) ?? new CatalogEntity();
        }

        public List<CatalogEntity> GetAllCatalogs()
        {
            return CatalogEntity.Select.ToList();
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
                "删除失败".Log<CatalogService>(LogLevel.Error, e);
                return false;
            }
            
            return true;
        }
    }
}