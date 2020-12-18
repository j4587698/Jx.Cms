using System.Linq;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Themes.Util;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Jx.Cms.Themes
{
    public class ResponsivePageRouteModelConvention: IPageRouteModelConvention
    {
        public void Apply(PageRouteModel model)
        {
            var area = model.AreaName;
            // 不处理区域内的
            if (!area.IsNullOrEmpty())
            {
                return;
            }

            var path = model.ViewEnginePath;
            // 第一个字符是/，所以从第二个字符开始找，第一段即为themeName
            var themeNameIndex = path.IndexOf('/', 1);
            // 如果没有/，证明不是主题类
            if (themeNameIndex == -1)
            {
                return;
            }

            var themeName = path.Substring(1, themeNameIndex - 1);
            if (!Utils.ThemePathDic.ContainsKey(themeName))
            {
                Utils.ThemePathDic.Add(themeName, model.RelativePath);
            }
            foreach (var selector in model.Selectors)
            {
                if (selector.AttributeRouteModel.Template == themeName)
                {
                    selector.AttributeRouteModel.Template = "/";
                    selector.EndpointMetadata.Add(new ThemeNameAttribute(themeName));
                    continue;
                }
                var templatePath = selector.AttributeRouteModel.Template.Substring(
                    selector.AttributeRouteModel.Template.IndexOf('/'));
                selector.AttributeRouteModel.Template = templatePath;
                selector.EndpointMetadata.Add(new ThemeNameAttribute(themeName));
            }
        }
    }
}