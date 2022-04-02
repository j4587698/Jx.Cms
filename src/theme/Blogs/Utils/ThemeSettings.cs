using Blogs.Enum;
using Blogs.Model;
using Furion;
using Jx.Cms.Plugin.Service.Both;
using Masuit.Tools.Reflection;
using Masuit.Tools.Systems;

namespace Blogs.Utils
{
    public static class ThemeSettings
    {
        public static SettingsModel GetSettings()
        {
            var settingsService = App.GetService<ISettingsService>();
            var settingsEnumerable = settingsService.GetAllValues("Blogs");
            var model = new SettingsModel();
            foreach (var settings in settingsEnumerable)
            {
                if (settings.Key == "Layout")
                {
                    model.SetProperty(settings.Key, (BlogLayoutEnum)typeof(BlogLayoutEnum).GetValue(settings.Value));
                }
                else if (settings.Key == "Sidebar")
                {
                    model.SetProperty(settings.Key, (SidebarEnum)typeof(SidebarEnum).GetValue(settings.Value));
                }
                else
                {
                    model.SetProperty(settings.Key, settings.Value??"");
                }
            }

            return model;
        }
        
        public static void SaveSettings(SettingsModel settingsModel)
        {
            var settingsService = App.GetService<ISettingsService>();
            var properties = settingsModel.GetProperties();
            foreach (var property in properties)
            {
                settingsService.SetValue("Blogs", property.Name, property.GetValue(settingsModel)?.ToString());
            }
        }
    }
}