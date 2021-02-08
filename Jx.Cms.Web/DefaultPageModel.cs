using System.Linq;
using Furion;
using Jx.Cms.Common.Utils;
using Jx.Cms.Service.Both;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Jx.Cms.Web
{
    public class DefaultPageModel: PageModel
    {

        private readonly ISettingsService _settingsService;

        public DefaultPageModel()
        {
            _settingsService = App.GetService<ISettingsService>();
        }
        
        public override void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            base.OnPageHandlerSelected(context);
            var settings = _settingsService.GetValuesByNames(new[] {SettingsConstants.TitleKey, 
                SettingsConstants.SubTitleKey, SettingsConstants.UrlKey, SettingsConstants.CopyRightKey}).ToList();
            foreach (var setting in settings)
            {
                ViewData[setting.name] = setting.value;
            }
        }
    }
}