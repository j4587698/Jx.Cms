using Jx.Cms.Common.Vo;
using Jx.Cms.DbContext.Entities.Article;
using Mapster;
using Microsoft.AspNetCore.Hosting;

namespace Jx.Cms.Plugin.Mapper;

public class MediaMapper : IRegister
{
    private readonly IWebHostEnvironment _hostingEnvironment;

    public MediaMapper(IWebHostEnvironment hostingEnvironment)
    {
        _hostingEnvironment = hostingEnvironment;
    }

    public void Register(TypeAdapterConfig config)
    {
        config.ForType<MediaEntity, MediaInfoVo>()
            .Map(dest => dest.FullPath,
                src => Path.Combine(_hostingEnvironment.WebRootPath, src.Url.TrimStart('/')))
            .AfterMapping((src, dest) => dest.MediaInfo = new FileInfo(dest.FullPath));
    }
}