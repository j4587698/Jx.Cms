using System.Collections.Generic;
using System.Linq;
using Furion.DependencyInjection;
using Jx.Cms.DbContext.Entities.Article;

namespace Jx.Cms.DbContext.Service.Both.Impl
{
    public class TagService: ITagService, ITransient
    {
        public TagEntity GetLabelById(int id)
        {
            return TagEntity.Find(id);
        }

        public List<TagEntity> LabelNameToLabels(List<string> labelNames)
        {
            return TagEntity.Select.Where(x => labelNames.Contains(x.Name)).ToList();
        }

        public List<TagEntity> AllLabelNameToLabels(List<string> labelNames)
        {
            var labels = LabelNameToLabels(labelNames);
            var existLabels = labels.Select(x => x.Name);
            labels.AddRange(labelNames.Except(existLabels).Select(x => new TagEntity() {Name = x}));
            return labels;
        }

        public List<ArticleEntity> GetArticleFormLabelId(int id, int pageNumber, int pageSize)
        {
            return ArticleEntity.Select.Where(x => x.Labels.AsSelect().Any(y => y.Id == id)).Page(pageNumber, pageSize).Include(x => x.Catalogue).ToList();
        }

        public List<ArticleEntity> GetArticleFormLabelId(int id, int pageNumber, int pageSize, out long count)
        {
            return ArticleEntity.Select.Where(x => x.Labels.AsSelect().Any(y => y.Id == id)).Count(out count).Page(pageNumber, pageSize).Include(x => x.Catalogue).ToList();
        }

        public List<ArticleEntity> GetArticleFormLabelName(string name)
        {
            return TagEntity.Select.IncludeMany(x => x.Articles).Where(x => x.Name == name).First()?.Articles;
        }
    }
}