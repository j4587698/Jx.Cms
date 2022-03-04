using System;
using System.ComponentModel;
using FreeSql;
using FreeSql.DataAnnotations;
using Jx.Cms.Common.Enum;

namespace Jx.Cms.Entities.Settings;

[Description("小工具表")]
public class WidgetSettingsEntity : BaseEntity<WidgetSettingsEntity, Guid>
{
    [Description("小工具名")]
    public string Name { get; set; }

    [Description("所在菜单")]
    public WidgetMenuType WidgetMenuType { get; set; }

    [Description("键")]
    public string Key { get; set; }

    [Description("值")]
    [Column(StringLength = 2000)]
    public string Value { get; set; }
}