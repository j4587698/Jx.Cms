using System.Linq;
using Jx.Cms.Admin.ViewModel;
using Jx.Cms.Entities.Article;
using Jx.Cms.Service.Admin;
using Mapster;

namespace Jx.Cms.Admin.Mapper
{
    public class AdminMapper: IRegister
    {
        private readonly ILabelService _labelService;

        public AdminMapper()
        {
            _labelService = Furion.App.GetService<ILabelService>();
        }

        public void Register(TypeAdapterConfig config)
        {
            config.ForType<ArticleEntity, ArticleVm>()
                .Map(dest => dest.Labels, src => src.Labels == null ? "" : string.Join(",",src.Labels.Select(x => x.Name)));
            config.ForType<ArticleVm, ArticleEntity>().Map(dest => dest.Labels, src => src.Labels == null ? null:_labelService.AllLabelNameToLabels(src.Labels.Split(new []{','}).ToList()));
        }
    }
}