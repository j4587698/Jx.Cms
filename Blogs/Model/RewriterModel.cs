using Furion;
using Jx.Cms.Service.Both;
using Masuit.Tools.Reflection;

namespace Blogs.Model
{
    public class RewriterModel
    {
        public string RewriteOption { get; set; }

        public string ArticleUrl { get; set; }

        public string PageUrl { get; set; }

        public string IndexUrl { get; set; }


        private static RewriterModel _rewriterModel;
        
        public static RewriterModel GetSettings()
        {
            if (_rewriterModel == null)
            {
                var settingsService = App.GetService<ISettingsService>();
                var settingsEnumerable = settingsService.GetAllValues("Rewriter");
                _rewriterModel = new RewriterModel();
                foreach (var settings in settingsEnumerable)
                {

                    _rewriterModel.SetProperty(settings.Key, settings.Value??"");
                }
            }
            
            return _rewriterModel;
        }
        
        public static void SaveSettings(RewriterModel rewriterModel)
        {
            _rewriterModel = rewriterModel;
            var settingsService = App.GetService<ISettingsService>();
            var properties = rewriterModel.GetProperties();
            foreach (var property in properties)
            {
                settingsService.SetValue("Rewriter", property.Name, property.GetValue(rewriterModel)?.ToString());
            }
        }
    }
}