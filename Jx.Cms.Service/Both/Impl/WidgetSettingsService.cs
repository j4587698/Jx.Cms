using System.Collections.Generic;
using System.Linq;
using FreeSql;
using Furion.DependencyInjection;
using Jx.Cms.Common.Enum;
using Jx.Cms.Entities.Settings;

namespace Jx.Cms.Service.Both.Impl;

public class WidgetSettingsService: IWidgetSettingsService, ITransient
{
    public Dictionary<string, string> GetAll(string widgetName, WidgetMenuType widgetMenuType)
    {
        return WidgetSettingsEntity.Where(x => x.Name == widgetName && x.WidgetMenuType == widgetMenuType).ToDictionary(x => x.Key, y => y.Value);
    }

    public string GetValue(string widgetName,WidgetMenuType widgetMenuType, string key)
    {
        return WidgetSettingsEntity.Where(x => x.Name == widgetName && x.Key == key && x.WidgetMenuType == widgetMenuType).First(x => x.Value);
    }

    public void SetValue(string widgetName, WidgetMenuType widgetMenuType, string key, string value)
    {
        var widgetEntity = WidgetSettingsEntity.Where(x => x.Name == widgetName && x.Key == key && x.WidgetMenuType == widgetMenuType).First() ?? new WidgetSettingsEntity
        {
            Name = widgetName,
            Key = key,
            WidgetMenuType = widgetMenuType
        };
        widgetEntity.Value = value;
        widgetEntity.Save();
    }

    public void SetValues(string widgetName, WidgetMenuType widgetMenuType, Dictionary<string, string> values)
    {
        var widgets = WidgetSettingsEntity.Where(x => x.Name == widgetName && x.WidgetMenuType == widgetMenuType).ToList();
        BaseEntity.Orm.Transaction(() =>
        {
            foreach (var value in values)
            {
                var widget = widgets.FirstOrDefault(x => x.Key == value.Key) ?? new WidgetSettingsEntity()
                {
                    Name = widgetName,
                    Key = value.Key,
                    WidgetMenuType = widgetMenuType
                };

                widget.Value = value.Value;
                widget.Save();
            }
        });
    }
}