using Jx.Cms.Common.Extensions;
using Jx.Cms.Plugin.Service.Both;
using Jx.Toolbox.Extensions;
using PulseBlog.Enum;
using PulseBlog.Model;

namespace PulseBlog.Utils;

public static class ThemeSettings
{
    public static SettingsModel GetSettings()
    {
        var settingsService = ServicesExtension.GetRequiredService<ISettingsService>();
        var values = settingsService.GetAllValues("PulseBlog");
        var model = new SettingsModel();
        foreach (var kv in values)
        {
            if (kv.Key == nameof(SettingsModel.Sidebar))
            {
                model.SetProperty(kv.Key, (kv.Value ?? "").ToEnum<SidebarLayoutEnum>());
                continue;
            }

            model.SetProperty(kv.Key, kv.Value ?? "");
        }

        model.AccentColor = ThemeViewHelper.NormalizeColor(model.AccentColor);
        return model;
    }

    public static void SaveSettings(SettingsModel model)
    {
        var settingsService = ServicesExtension.GetRequiredService<ISettingsService>();
        model.AccentColor = ThemeViewHelper.NormalizeColor(model.AccentColor);
        var properties = model.GetType().GetProperties();
        foreach (var property in properties)
        {
            settingsService.SetValue("PulseBlog", property.Name, property.GetValue(model)?.ToString());
        }
    }
}
