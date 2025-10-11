using System.IO;
using Microsoft.AspNetCore.Hosting;
using Jx.Cms.Common.Vo;
using Jx.Cms.DbContext.Entities.Article;
using Mapster;

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