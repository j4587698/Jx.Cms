using System.ComponentModel;
using FreeSql;
using Jx.Cms.Common.Enum;

namespace Jx.Cms.DbContext.Entities.Article;

[Description("媒体类")]
public class MediaEntity : BaseEntity<MediaEntity, long>
{
    [Description("实际地址")]
    public string Url { get; set; }

    [Description("上传时的文件名")]
    public string Name { get; set; }

    [Description("描述信息")]
    public string Alt { get; set; }

    [Description("媒体类型")]
    public MediaTypeEnum MediaType { get; set; }
}