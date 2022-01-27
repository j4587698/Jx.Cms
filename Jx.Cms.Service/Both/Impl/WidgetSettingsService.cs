using System.Collections.Generic;
using System.Linq;
using FreeSql;
using Furion.DependencyInjection;
using Jx.Cms.Entities.Settings;

namespace Jx.Cms.Service.Both.Impl;

public class WidgetSettingsService: IWidgetSettingsService, ITransient
{
    public Dictionary<string, string> GetAll(string widgetName)
    {
        return WidgetSettingsEntity.Where(x => x.Name == widgetName).ToDictionary(x => x.Key, y => y.Value);
    }

    public string GetValue(string widgetName, string key)
    {
        return WidgetSettingsEntity.Where(x => x.Name == widgetName && x.Key == key).First(x => x.Value);
    }

    public void SetValue(string widgetName, string key, string value)
    {
        var widgetEntity = WidgetSettingsEntity.Where(x => x.Name == widgetName && x.Key == key).First() ?? new WidgetSettingsEntity
        {
            Name = widgetName,
            Key = key
        };
        widgetEntity.Value = value;
        widgetEntity.Save();
    }

    public void SetValues(string widgetName, Dictionary<string, string> values)
    {
        var widgets = WidgetSettingsEntity.Where(x => x.Name == widgetName).ToList();
        BaseEntity.Orm.Transaction(() =>
        {
            foreach (var value in values)
            {
                var widget = widgets.FirstOrDefault(x => x.Key == value.Key);
                if (widget == null)
                {
                    widget = new WidgetSettingsEntity()
                    {
                        Name = widgetName,
                        Key = value.Key
                    };
                }

                widget.Value = value.Value;
                widget.Save();
            }
        });
        
        
    }
}