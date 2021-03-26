using System;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Furion;
using HighlightingPlugin.Pages;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Plugin;

namespace HighlightingPlugin
{
    public class ArticleInstance : IArticlePlugin
    {
        public ArticleModel OnArticleShow(ArticleModel articleModel)
        {
            articleModel.Body.Content += "Highlighting Run";
            articleModel.Header += "<link rel=\"stylesheet\" href=\"/highlight/styles/default.css\">";
            articleModel.Footer += "<script src=\"/highlight/highlight.pack.js\"></script><script>hljs.highlightAll();</script>";
            return articleModel;
        }

        public EditorExtModel AddEditorToolbarButton()
        {
            var dialogService = App.GetService<SwalService>();
            
            EditorExtModel extModel = new EditorExtModel();
            extModel.ToolbarButton = new EditorToolbarButton()
            {
                ButtonName = "AddCode",
                IconClass = "fa fa-code",
                Tooltip = $"{(dialogService == null ? "空dialogService": $"{dialogService.GetType()}")}"
            };
            extModel.OnToolbarClick = async pluginName =>
            {
                await dialogService.Show(new SwalOption()
                {
                    IsConfirm = true,
                    Content = "这是一个提示"
                });
                return pluginName;
            };
            return extModel;
        }
    }
}