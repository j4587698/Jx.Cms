using System.Collections.Generic;
using System.Linq;
using FreeSql;
using Furion.DependencyInjection;
using Jx.Cms.Common.Enum;
using Jx.Cms.Entities.Settings;

namespace Jx.Cms.Service.Both.Impl;

public class WidgetSettingsService: IWidgetSettingsService, ITransient
{
    public Dictionary<string, string> GetAll(string widgetName, WidgetSidebarType widgetSidebarType)
    {
        return WidgetSettingsEntity.Where(x => x.Name == widgetName && x.WidgetSidebarType == widgetSidebarType).ToDictionary(x => x.Key, y => y.Value);
    }

    public string GetValue(string widgetName,WidgetSidebarType widgetSidebarType, string key)
    {
        return WidgetSettingsEntity.Where(x => x.Name == widgetName && x.Key == key && x.WidgetSidebarType == widgetSidebarType).First(x => x.Value);
    }

    public void SetValue(string widgetName, WidgetSidebarType widgetSidebarType, string key, string value)
    {
        var widgetEntity = WidgetSettingsEntity.Where(x => x.Name == widgetName && x.Key == key && x.WidgetSidebarType == widgetSidebarType).First() ?? new WidgetSettingsEntity
        {
            Name = widgetName,
            Key = key,
            WidgetSidebarType = widgetSidebarType
        };
        widgetEntity.Value = value;
        widgetEntity.Save();
    }

    public void SetValues(string widgetName, WidgetSidebarType widgetSidebarType, Dictionary<string, string> values)
    {
        var widgets = WidgetSettingsEntity.Where(x => x.Name == widgetName && x.WidgetSidebarType == widgetSidebarType).ToList();
        BaseEntity.Orm.Transaction(() =>
        {
            foreach (var value in values)
            {
                var widget = widgets.FirstOrDefault(x => x.Key == value.Key) ?? new WidgetSettingsEntity()
                {
                    Name = widgetName,
                    Key = value.Key,
                    WidgetSidebarType = widgetSidebarType
                };

                widget.Value = value.Value;
                widget.Save();
            }
        });
    }
}