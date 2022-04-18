using System.Collections.Generic;
using System.Linq;
using Furion.DependencyInjection;
using Jx.Cms.DbContext.Entities.Article;

namespace Jx.Cms.Plugin.Service.Both.Impl
{
    public class TagService: ITagService, ITransient
    {
        public TagEntity GetTagById(int id)
        {
            return TagEntity.Find(id);
        }

        public List<TagEntity> GetAllTags()
        {
            return TagEntity.Select.ToList();
        }

        public List<TagEntity> TagNameToTags(List<string> tagNames)
        {
            return TagEntity.Select.Where(x => tagNames.Contains(x.Name)).ToList();
        }

        public List<TagEntity> AllTagNameToTags(List<string> tagNames)
        {
            var labels = TagNameToTags(tagNames);
            var existLabels = labels.Select(x => x.Name);
            labels.AddRange(tagNames.Except(existLabels).Select(x => new TagEntity() {Name = x}));
            return labels;
        }

        public List<ArticleEntity> GetArticleFromTagId(int id, int pageNumber, int pageSize)
        {
            return ArticleEntity.Select.Where(x => x.Tags.AsSelect().Any(y => y.Id == id)).Page(pageNumber, pageSize).Include(x => x.Catalogue).ToList();
        }

        public List<ArticleEntity> GetArticleFromTagId(int id, int pageNumber, int pageSize, out long count)
        {
            return ArticleEntity.Select.Where(x => x.Tags.AsSelect().Any(y => y.Id == id)).Count(out count).Page(pageNumber, pageSize).Include(x => x.Catalogue).ToList();
        }

        public List<ArticleEntity> GetArticleFromTagName(string name)
        {
            return TagEntity.Select.IncludeMany(x => x.Articles).Where(x => x.Name == name).First()?.Articles;
        }
    }
}