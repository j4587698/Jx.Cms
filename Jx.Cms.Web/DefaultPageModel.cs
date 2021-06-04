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
        private readonly IMenuService _menuService;

        public DefaultPageModel()
        {
            _settingsService = App.GetService<ISettingsService>();
            _menuService = App.GetService<IMenuService>();
        }
        
        public override void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            base.OnPageHandlerSelected(context);
            var settings = _settingsService.GetValuesByNames(new[]
            {
                SettingsConstants.TitleKey,
                SettingsConstants.SubTitleKey, SettingsConstants.UrlKey, SettingsConstants.CopyRightKey
            });
            foreach (var setting in settings)
            {
                ViewData[setting.Key] = setting.Value;
            }

            ViewData["menu"] = _menuService.GetAllMenuTree();
        }
    }
}