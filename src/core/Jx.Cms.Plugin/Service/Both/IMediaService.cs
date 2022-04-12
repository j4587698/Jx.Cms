using System.Collections.Generic;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Jx.Cms.Common.Enum;
using Jx.Cms.DbContext.Entities.Article;

namespace Jx.Cms.Plugin.Service.Both;

public interface IMediaService
{
    /// <summary>
    /// 添加媒体文件
    /// </summary>
    /// <param name="file"></param>
    Task<bool> AddMediaAsync(UploadFile file);

    /// <summary>
    /// 获取所有的媒体文件
    /// </summary>
    /// <returns></returns>
    IEnumerable<MediaEntity> GetAllMedias();

    /// <summary>
    /// 根据媒体类型获取媒体文件
    /// </summary>
    /// <returns></returns>
    IEnumerable<MediaEntity> GetMediasByMediaType(MediaTypeEnum mediaTypeEnum);

    /// <summary>
    /// 根据扩展名获取媒体文件
    /// </summary>
    /// <param name="extensions"></param>
    /// <returns></returns>
    IEnumerable<MediaEntity> GetMediasByExtensions(IEnumerable<string> extensions);

    /// <summary>
    /// 修改媒体文件描述
    /// </summary>
    /// <param name="media"></param>
    void ModifyMedia(MediaEntity media);
}