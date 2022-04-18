using System.Linq;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.Plugin.Service.Both;
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
                .Map(dest => dest.Tags, src => src.Tags == null ? null : src.Tags.Select(x => x.Name));
            config.ForType<ArticleVm, ArticleEntity>().Map(dest => dest.Tags, src => src.Tags == null ? null:_tagService.AllTagNameToTags(src.Tags));
        }
    }
}