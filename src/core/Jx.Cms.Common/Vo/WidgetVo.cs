using System;

namespace Jx.Cms.Common.Vo;

/// <summary>
/// 小工具Vo
/// </summary>
public class WidgetVo
{
    /// <summary>
    /// 小工具Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 小工具名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 小工具参数
    /// </summary>
    public string Parameter { get; set; }
}