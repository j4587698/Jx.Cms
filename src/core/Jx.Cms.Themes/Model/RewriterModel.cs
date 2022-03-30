using Furion;
using Jx.Cms.DbContext.Service.Both;
using Masuit.Tools.Reflection;

namespace Jx.Cms.Themes.Model
{
    public class RewriterModel
    {
        public string RewriteOption { get; set; }

        public string ArticleUrl { get; set; }

        public string PageUrl { get; set; }

        public string IndexUrl { get; set; }

        public string CatalogueUrl { get; set; }

        public string LabelUrl { get; set; }
        
        public string DateUrl { get; set; }


        private static RewriterModel _rewriterModel;
        
        public static RewriterModel GetSettings()
        {
            if (_rewriterModel == null)
            {
                var settingsService = App.GetService<ISettingsService>();
                var settingsEnumerable = settingsService.GetAllValues("Rewriter");
                _rewriterModel = new RewriterModel();
                var prop = _rewriterModel.GetProperties();
                var index = prop[0].GetIndexParameters();
                foreach (var settings in settingsEnumerable)
                {
                    _rewriterModel.SetProperty(settings.Key, settings.Value??"");
                    //prop[0].SetValue(_rewriterModel, settings.Value);
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