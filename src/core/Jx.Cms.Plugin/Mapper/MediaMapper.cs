using System.IO;
using Furion;
using Jx.Cms.Common.Vo;
using Jx.Cms.DbContext.Entities.Article;
using Mapster;

namespace Jx.Cms.Plugin.Mapper;

public class MediaMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<MediaEntity, MediaInfoVo>()
            .Map(dest => dest.FullPath,
                src => Path.Combine(App.WebHostEnvironment.WebRootPath, src.Url.TrimStart('/')))
            .AfterMapping((src, dest) => dest.MediaInfo = new FileInfo(dest.FullPath));
    }
}