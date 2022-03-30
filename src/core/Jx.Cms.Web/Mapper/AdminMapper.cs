using System.Linq;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.DbContext.Service.Both;
using Jx.Cms.Web.ViewModel;
using Mapster;

namespace Jx.Cms.Web.Mapper
{
    public class AdminMapper: IRegister
    {
        private readonly ITagService _tagService;

        public AdminMapper()
        {
            _tagService = Furion.App.GetService<ITagService>();
        }

        public void Register(TypeAdapterConfig config)
        {
            config.ForType<ArticleEntity, ArticleVm>()
                .Map(dest => dest.Labels, src => src.Labels == null ? "" : string.Join(",",src.Labels.Select(x => x.Name)));
            config.ForType<ArticleVm, ArticleEntity>().Map(dest => dest.Labels, src => src.Labels == null ? null:_tagService.AllLabelNameToLabels(src.Labels.Split(new []{','}).ToList()));
        }
    }
}