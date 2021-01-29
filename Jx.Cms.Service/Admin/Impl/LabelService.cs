using System.Collections.Generic;
using System.Linq;
using Furion.DependencyInjection;
using Jx.Cms.Entities.Article;

namespace Jx.Cms.Service.Admin.Impl
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
    }
}