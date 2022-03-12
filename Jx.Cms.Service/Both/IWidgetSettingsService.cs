using System.Collections.Generic;
using Jx.Cms.Common.Enum;

namespace Jx.Cms.Service.Both;

/// <summary>
/// 小工具
/// </summary>
public interface IWidgetSettingsService
{
    /// <summary>
    /// 获取小工具的所有属性
    /// </summary>
    /// <param name="widgetName">小工具名</param>
    /// <param name="widgetSidebarType">小工具所在菜单</param>
    /// <returns></returns>
    Dictionary<string, string> GetAll(string widgetName, WidgetSidebarType widgetSidebarType);

    /// <summary>
    /// 根据小工具名和key获取value
    /// </summary>
    /// <param name="widgetName">小工具名</param>
    /// <param name="widgetSidebarType">小工具所在菜单</param>
    /// <param name="key">key</param>
    /// <returns></returns>
    string GetValue(string widgetName, WidgetSidebarType widgetSidebarType, string key);

    /// <summary>
    /// 设置小工具的值
    /// </summary>
    /// <param name="widgetName">小工具名</param>
    /// <param name="widgetSidebarType">小工具所在菜单</param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    void SetValue(string widgetName, WidgetSidebarType widgetSidebarType, string key, string value);

    /// <summary>
    /// 批量设置小工具的值
    /// </summary>
    /// <param name="widgetName">小工具名</param>
    /// <param name="widgetSidebarType">小工具所在菜单</param>
    /// <param name="values"></param>
    void SetValues(string widgetName, WidgetSidebarType widgetSidebarType, Dictionary<string, string> values);
}