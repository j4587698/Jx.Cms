using System;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Furion;
using Furion.DependencyInjection.Extensions;
using HighlightingPlugin.Pages;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Plugin;

namespace HighlightingPlugin
{
    public class ArticleInstance : IArticlePlugin
    {
        public ArticleModel OnArticleShow(ArticleModel articleModel)
        {
            articleModel.Header += "<link rel=\"stylesheet\" href=\"/highlight/styles/default.css\">";
            articleModel.Footer += "<script src=\"/highlight/highlight.pack.js\"></script><script>hljs.highlightAll();</script>";
            return articleModel;
        }

        public EditorExtModel AddEditorToolbarButton(DialogService dialogService)
        {
            EditorExtModel extModel = new EditorExtModel();
            extModel.ToolbarButton = new EditorToolbarButton()
            {
                ButtonName = "AddCode",
                IconClass = "fa fa-code",
                Tooltip = "添加代码段"
            };
            extModel.OnDialogCreat = option =>
            {
                option.Title = "添加代码段";
                option.ButtonYesText = "插入代码段";
                return typeof(HighlightingDialog);
            };
            extModel.OnToolbarClick = result =>
            {
                if (result.dialogResult == DialogResult.Yes)
                {
                    var highlightDialog = result.pluginDialog as HighlightingDialog;
                    if (highlightDialog == null)
                    {
                        return Task.FromResult("");
                    }

                    if (highlightDialog.SelectedValue == "Auto")
                    {
                        return Task.FromResult<string>($"<pre><code>{HtmlToEsc(highlightDialog.CodeValue)}</code></pre>");
                    }
                    return Task.FromResult<string>($"<pre><code class=\"{highlightDialog.SelectedValue.ToLower()}\">{HtmlToEsc(highlightDialog.CodeValue)}</code></pre>");
                }
                return Task.FromResult<string>("");
            };
            return extModel;
        }
        
        /// <summary>
        /// Html to Esc
        /// </summary>
        /// <param name="input">input</param>
        /// <returns></returns>
        public static string HtmlToEsc(string input)
        {
            if (string.IsNullOrEmpty(input)) { return ""; }
 
            input = input.Replace("&", "&amp;")
                .Replace("'", "&#39;")
                .Replace("\"", "&quot;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace(" ", "&nbsp;")
                .Replace("©", "&copy;")
                .Replace("®", "&reg;")
                .Replace("™", "&#8482;");
            return input;
        }
    }
}