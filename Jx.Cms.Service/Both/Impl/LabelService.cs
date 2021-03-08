using System.Collections.Generic;
using System.Linq;
using Furion.DependencyInjection;
using Jx.Cms.Entities.Article;
using Jx.Cms.Service.Admin;

namespace Jx.Cms.Service.Both.Impl
{
    public class LabelService: ILabelService, ITransient
    {
        public List<LabelEntity> LabelNameToLabels(List<string> labelNames)
        {
            return LabelEntity.Select.Where(x => labelNames.Contains(x.Name)).ToList();
        }

        public List<LabelEntity> AllLabelNameToLabels(List<string> labelNames)
        {
            var labels = LabelNameToLabels(labelNames);
            var existLabels = labels.Select(x => x.Name);
            labels.AddRange(labelNames.Except(existLabels).Select(x => new LabelEntity() {Name = x}));
            return labels;
        }

        public List<ArticleEntity> GetArticleFormLabelId(int id, int pageNumber, int pageSize)
        {
            return ArticleEntity.Select.Where(x => x.Labels.AsSelect().Any(y => y.Id == id)).Page(pageNumber, pageSize).ToList();
        }

        public List<ArticleEntity> GetArticleFormLabelId(int id, int pageNumber, int pageSize, out long count)
        {
            return ArticleEntity.Select.Where(x => x.Labels.AsSelect().Any(y => y.Id == id)).Count(out count).Page(pageNumber, pageSize).ToList();
        }

        public List<ArticleEntity> GetArticleFormLabelName(string name)
        {
            return LabelEntity.Select.IncludeMany(x => x.Articles).Where(x => x.Name == name).First()?.Articles;
        }
    }
}