using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BootstrapBlazor.Components;
using Furion;
using Furion.DependencyInjection;
using Jx.Cms.Common.Enum;
using Jx.Cms.DbContext.Entities.Article;
using Masuit.Tools;
using Masuit.Tools.AspNetCore.Mime;

namespace Jx.Cms.Plugin.Service.Both.Impl;

public class MediaService : IMediaService, ITransient
{
    public void AddMedia(UploadFile file)
    {
        var urlBase = Path.Combine("upload", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));
        var dir = Path.Combine(App.WebHostEnvironment.WebRootPath, urlBase);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        var fileName = Stopwatch.GetTimestamp().ToBinary(8) + Path.GetExtension(file.OriginFileName);
        file.SaveToFile(Path.Combine(dir, fileName));
        MediaEntity mediaEntity = new MediaEntity();
        mediaEntity.Url = Path.Combine("/", urlBase, fileName).Replace('\\', '/');
        MimeMapper mapper = new MimeMapper();
        var mime = mapper.GetMimeFromExtension(Path.GetExtension(fileName)).Split('/')[0];
        mediaEntity.Name = file.OriginFileName;
        mediaEntity.MediaType = mime switch
        {
            "image" => MediaTypeEnum.Image,
            "audio" => MediaTypeEnum.Audio,
            "video" => MediaTypeEnum.Video,
            _ => MediaTypeEnum.UnKnow
        };
        mediaEntity.Save();
    }

    public IEnumerable<MediaEntity> GetAllMedias()
    {
        return MediaEntity.Select.OrderByDescending(x => x.CreateTime).ToList();
    }

    public void ModifyMedia(MediaEntity media)
    {
        media.Save();
    }
}