using BootstrapBlazor.Components;
using HighlightingPlugin.Pages;
using Jx.Cms.DbContext.Entities.Article;
using Jx.Cms.Plugin.Model;
using Jx.Cms.Plugin.Plugin;
using Jx.Cms.Plugin.Service.Both;
using Microsoft.AspNetCore.Components;

namespace HighlightingPlugin;

public class ArticleInstance : IArticlePlugin
{
    private readonly ISettingsService _settingsService;

    private const string HighlightAssetVersion = "11.11.1-20260306.2";

    public ArticleInstance()
    {
    }

    public ArticleInstance(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public RenderFragment<ArticleEntity> BottomRender { get; } = article => builder =>
    {
        builder.OpenComponent<Settings>(0);

        builder.CloseComponent();
    };

    public ArticleModel OnArticleShow(ArticleModel articleModel)
    {
        var themeStyle = HighlightStyleHelper.Normalize(
            _settingsService?.GetValue(Constant.SettingsKey, Constant.ThemeStyle) ?? HighlightStyleHelper.DefaultStyleFile);
        var enableInlineCode = !bool.TryParse(
                                   _settingsService?.GetValue(Constant.SettingsKey, Constant.EnableInlineCode),
                                   out var value) ||
                               value;

        articleModel.Header +=
            $"<link rel=\"stylesheet\" href=\"/highlight/styles/{themeStyle}?v={HighlightAssetVersion}\">";
        articleModel.Header += $"<link rel=\"stylesheet\" href=\"/highlight/jx-highlight.css?v={HighlightAssetVersion}\">";
        articleModel.Footer +=
            $"<script>window.jxHighlightOptions={{enableInlineCode:{(enableInlineCode ? "true" : "false")}}};</script>";
        articleModel.Footer += $"<script src=\"/highlight/highlight.pack.js?v={HighlightAssetVersion}\"></script>";
        articleModel.Footer += $"<script src=\"/highlight/jx-highlight.js?v={HighlightAssetVersion}\"></script>";
        return articleModel;
    }

    public void OnArticleSaved(ArticleEntity articleEntity)
    {
    }

    public EditorExtModel AddEditorToolbarButton(DialogService dialogService)
    {
        var extModel = new EditorExtModel();
        extModel.ToolbarButton = new EditorToolbarButton
        {
            ButtonName = "AddCode",
            IconClass = "fa fa-code",
            Tooltip = "添加代码段"
        };
        extModel.OnDialogCreate = option =>
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
                if (highlightDialog == null) return Task.FromResult("");

                if (highlightDialog.SelectedValue == "Auto")
                    return Task.FromResult($"<pre><code>{HtmlToEsc(highlightDialog.CodeValue)}</code></pre>");
                return Task.FromResult(
                    $"<br><pre><code class=\"{highlightDialog.SelectedValue.ToLower()}\">{HtmlToEsc(highlightDialog.CodeValue)}</code></pre><br>");
            }

            return Task.FromResult("");
        };
        return extModel;
    }

    /// <summary>
    ///     Html to Esc
    /// </summary>
    /// <param name="input">input</param>
    /// <returns></returns>
    public static string HtmlToEsc(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";

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
