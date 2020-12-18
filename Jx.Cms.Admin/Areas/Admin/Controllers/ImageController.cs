using System.IO;
using System.Net.Mime;
using Jx.Cms.Themes.Service;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ImageController : Controller
    {
        private IThemeConfigService _themeConfigService;
        
        public ImageController(IThemeConfigService themeConfigService)
        {
            _themeConfigService = themeConfigService;
        }
        
        // GET
        public IActionResult LoadScreenShot(string themeName)
        {
            return File(_themeConfigService.GetScreenShotStreamByThemeName(themeName), "image/jpeg");
        }
    }
}