using System.Threading.Tasks;
using Jx.Cms.Rewrite;
using Jx.Cms.Themes.Model;
using Jx.Cms.Themes.Util;
using Microsoft.AspNetCore.Http;

namespace Jx.Cms.Themes.Middlewares;

public class RewriteMiddleware
{
    private readonly RequestDelegate _next;

    public RewriteMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var hasArea = context.Request.RouteValues.ContainsKey("area");
        if (hasArea)
        {
            await _next.Invoke(context);
            return;
        }
        var rewriterModel = RewriterModel.GetSettings();
        if (rewriterModel == null || rewriterModel.RewriteOption == RewriteOptionEnum.Dynamic.ToString())
        {
            await _next.Invoke(context);
            return;
        }

        var settings = RewriterModel.GetSettings();
        var url = RewriteUtil.AnalysisArticle(context.Request.Path, settings);
        if (url != null)
        {
            context.Request.Path = "/Post";
            context.Request.QueryString = new QueryString(url);
            await _next.Invoke(context);
            return;
        }

        url = RewriteUtil.AnalysisPage(context.Request.Path, settings);
        if (url != null)
        {
            context.Request.Path = "/Page";
            context.Request.QueryString = new QueryString(url);
            await _next.Invoke(context);
            return;
        }
        url = RewriteUtil.AnalysisIndex(context.Request.Path, settings);
        if (url != null)
        {
            context.Request.Path = "/";
            context.Request.QueryString = new QueryString(url);
            await _next.Invoke(context);
            return;
        }

        url = RewriteUtil.AnalysisCatalog(context.Request.Path, settings);
        if (url != null)
        {
            context.Request.Path = "/Catalogue";
            context.Request.QueryString = new QueryString(url);
            await _next.Invoke(context);
            return;
        }

        url = RewriteUtil.AnalysisTag(context.Request.Path, settings);
        if (url != null)
        {
            context.Request.Path = "/Tag";
            context.Request.QueryString = new QueryString(url);
            await _next.Invoke(context);
            return;
        }
        
        url = RewriteUtil.AnalysisDate(context.Request.Path, settings);
        if (url != null)
        {
            context.Request.Path = "/Date";
            context.Request.QueryString = new QueryString(url);
            await _next.Invoke(context);
            return;
        }
        
        await _next.Invoke(context);
    }
    
}