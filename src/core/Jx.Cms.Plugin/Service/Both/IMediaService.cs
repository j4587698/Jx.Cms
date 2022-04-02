using System.Collections.Generic;
using BootstrapBlazor.Components;
using Jx.Cms.DbContext.Entities.Article;

namespace Jx.Cms.Plugin.Service.Both;

public interface IMediaService
{
    /// <summary>
    /// 添加媒体文件
    /// </summary>
    /// <param name="file"></param>
    void AddMedia(UploadFile file);

    /// <summary>
    /// 获取所有的媒体文件
    /// </summary>
    /// <returns></returns>
    IEnumerable<MediaEntity> GetAllMedias();

    /// <summary>
    /// 修改媒体文件描述
    /// </summary>
    /// <param name="media"></param>
    void ModifyMedia(MediaEntity media);
}