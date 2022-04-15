﻿using System.Linq;
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
                .Map(dest => dest.Labels, src => src.Tags == null ? "" : string.Join(",",src.Tags.Select(x => x.Name)));
            config.ForType<ArticleVm, ArticleEntity>().Map(dest => dest.Tags, src => src.Labels == null ? null:_tagService.AllLabelNameToLabels(src.Labels.Split(new []{','}).ToList()));
        }
    }
}