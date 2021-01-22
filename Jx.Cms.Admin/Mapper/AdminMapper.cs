using System.Linq;
using Jx.Cms.Admin.ViewModel;
using Jx.Cms.Entities.Article;
using Mapster;

namespace Jx.Cms.Admin.Mapper
{
    public class AdminMapper: IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<ArticleEntity, ArticleVm>()
                .Map(dest => dest.Labels, src => src.Labels == null ? "" : string.Join(",",src.Labels.Select(x => x.Name)));
        }
    }
}