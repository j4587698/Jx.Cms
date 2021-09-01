using System.Collections.Generic;
using Blogs.Components;
using Blogs.Model;
using Blogs.Utils;
using BootstrapBlazor.Components;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Plugin;
using Jx.Cms.Rewrite;
using Microsoft.AspNetCore.Http;

namespace Blogs
{
    public class RewriterInstance: ISystemPlugin
    {
        public void PluginEnable()
        {
            var rewriterModel = RewriterModel.GetSettings();
            if (rewriterModel.RewriteOption == null)
            {
                rewriterModel.RewriteOption = RewriteOptionEnum.Dynamic.ToString();
                RewriterModel.SaveSettings(rewriterModel);
            }
        }

        public List<PluginMenuModel> AddMenuItem()
        {
            return new()
            {
                new PluginMenuModel()
                {
                    DisplayName = "Blogs主题伪静态设置",
                    Icon = "fa fa-code",
                    MenuId = "AB0050BE-9EC0-49B8-A2F6-0403880D9B23",
                    PluginBody = BootstrapDynamicComponent.CreateComponent<RewriteSettings>().Render()
                }
            };
        }

        public bool AddMiddleware(HttpContext context)
        {
            var rewriterModel = RewriterModel.GetSettings();
            if (rewriterModel == null || rewriterModel.RewriteOption == RewriteOptionEnum.Dynamic.ToString())
            {
                return true;
            }

            var url = RewriteUtil.AnalysisArticle(context.Request.Path);
            if (url != null)
            {
                context.Request.Path = "/Article";
                context.Request.QueryString = new QueryString(url);
                return true;
            }

            url = RewriteUtil.AnalysisPage(context.Request.Path);
            if (url != null)
            {
                context.Request.Path = "/Page";
                context.Request.QueryString = new QueryString(url);
                return true;
            }
            url = RewriteUtil.AnalysisIndex(context.Request.Path);
            if (url != null)
            {
                context.Request.Path = "/Index";
                context.Request.QueryString = new QueryString(url);
                return true;
            }
            return true;
        }
    }
}