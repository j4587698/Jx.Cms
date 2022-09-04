using Jx.Cms.Common.Extensions;
using Jx.Cms.Common.Utils;
using Jx.Cms.Themes.Service;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;

namespace Jx.Cms.Web.Admin.Controllers
{
    [Area("Admin")]
    public class ImageController : Controller
    {
        private readonly IThemeConfigService _themeConfigService;
        
        public ImageController(IThemeConfigService themeConfigService)
        {
            _themeConfigService = themeConfigService;
        }
        
        /// <summary>
        /// 获取主题截图
        /// </summary>
        /// <param name="themeName"></param>
        /// <returns></returns>
        public IActionResult LoadScreenShot(string themeName)
        {
            return File(_themeConfigService.GetScreenShotStreamByThemeName(themeName), "image/jpeg");
        }

        /// <summary>
        /// 加载本地头像
        /// </summary>
        /// <returns></returns>
        public IActionResult LoadLocalAvatar(string name)
        {
            string ch = name.IsNullOrEmpty() ? "空" : name.Substring(0, 1);
            return File(Util.StringToImage(ch, 45, 45, 20, Color.White, Color.Blue), "image/png");
        }
        
        
    }
}