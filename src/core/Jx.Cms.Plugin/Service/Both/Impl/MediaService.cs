using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Hosting;
using Jx.Cms.Common.Enum;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Toolbox.Utils;

namespace Jx.Cms.Plugin.Service.Both.Impl;

public class MediaService : IMediaService
{
    private readonly IWebHostEnvironment _hostingEnvironment;
    
    public MediaService(IWebHostEnvironment hostingEnvironment)
    {
        _hostingEnvironment = hostingEnvironment;
    }
    
    public async Task<bool> AddMediaAsync(UploadFile file)
    {
        var urlBase = Path.Combine("upload", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));
        var dir = Path.Combine(_hostingEnvironment.WebRootPath, urlBase);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        var fileName = NumberFormat.ToDecimalString(Stopwatch.GetTimestamp(), 36) + Path.GetExtension(file.OriginFileName);
        if (!await file.SaveToFile(Path.Combine(dir, fileName), 50L * 1024 * 1024 * 1024))
        {
            return false;
        }

        MediaEntity mediaEntity = new MediaEntity();
        mediaEntity.Url = Path.Combine("/", urlBase, fileName).Replace('\\', '/');
        var mime = Mime.GetTypeFormExtension(Path.GetExtension(fileName));
        mediaEntity.Name = file.OriginFileName;
        mediaEntity.MediaType = mime switch
        {
            "image" => MediaTypeEnum.Image,
            "audio" => MediaTypeEnum.Audio,
            "video" => MediaTypeEnum.Video,
            _ => MediaTypeEnum.UnKnow
        };
        mediaEntity.Save();
        return true;
    }

    public IEnumerable<MediaEntity> GetAllMedias()
    {
        return MediaEntity.Select.OrderByDescending(x => x.CreateTime).ToList();
    }

    public IEnumerable<MediaEntity> GetMediasByMediaType(MediaTypeEnum mediaTypeEnum)
    {
        return MediaEntity.Where(x => x.MediaType == mediaTypeEnum).OrderByDescending(x => x.CreateTime).ToList();
    }

    public IEnumerable<MediaEntity> GetMediasByExtensions(IEnumerable<string> extensions)
    {
        return GetAllMedias().Where(x => extensions.Contains(Path.GetExtension(x.Name)));
    }

    public void ModifyMedia(MediaEntity media)
    {
        media.Save();
    }
}