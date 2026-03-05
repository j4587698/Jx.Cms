using System.Diagnostics;
using BootstrapBlazor.Components;
using Jx.Cms.Common.Enum;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Toolbox.Utils;
using Microsoft.AspNetCore.Hosting;

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
        if (file == null) return false;

        var urlBase = Path.Combine("upload", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));
        var dir = Path.Combine(_hostingEnvironment.WebRootPath, urlBase);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        var originalFileName = file.OriginFileName ?? string.Empty;
        var extension = Path.GetExtension(originalFileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".bin";
        }

        var fileName = NumberFormat.ToDecimalString(Stopwatch.GetTimestamp(), 36) + extension;
        if (!await file.SaveToFileAsync(Path.Combine(dir, fileName), 50L * 1024 * 1024 * 1024))
        {
            if (string.IsNullOrWhiteSpace(file.Error))
            {
                file.Error = "文件保存失败，请检查磁盘空间或文件权限";
            }

            return false;
        }

        var mediaEntity = new MediaEntity();
        mediaEntity.Url = Path.Combine("/", urlBase, fileName).Replace('\\', '/');
        var mime = Mime.GetTypeFormExtension(Path.GetExtension(fileName));
        mediaEntity.Name = string.IsNullOrWhiteSpace(originalFileName) ? fileName : originalFileName;
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
        if (extensions == null) return GetAllMedias();

        var extensionSet = extensions
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.StartsWith(".") ? x : "." + x)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (extensionSet.Count == 0) return GetAllMedias();

        return GetAllMedias().Where(x =>
        {
            var ext = Path.GetExtension(x.Name ?? string.Empty);
            if (string.IsNullOrWhiteSpace(ext))
            {
                ext = Path.GetExtension(x.Url ?? string.Empty);
            }

            return !string.IsNullOrWhiteSpace(ext) && extensionSet.Contains(ext);
        });
    }

    public void ModifyMedia(MediaEntity media)
    {
        media.Save();
    }
}
