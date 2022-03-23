using System.Collections.Generic;
using Jx.Cms.Common.Enum;

namespace Jx.Cms.Plugin.Model;

/// <summary>
/// 文章页扩展内容
/// </summary>
public class ArticleExtModel
{
    /// <summary>
    /// 插件名
    /// </summary>
    public string PluginName { get; set; }

    /// <summary>
    /// 扩展名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 显示名
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// 扩展类型
    /// </summary>
    public ArticleExtTypeEnum ArticleExtTypeEnum { get; set; }

    /// <summary>
    /// 扩展的值
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// 扩展的选项，目前只有Select需要
    /// </summary>
    public List<string> Items { get; set; }
}