using System.Threading.Tasks;
using Jx.Cms.Rewrite.Model;
using Microsoft.AspNetCore.Http;

namespace Jx.Cms.Rewrite.Middlewares;

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

        url = RewriteUtil.AnalysisCatalogue(context.Request.Path, settings);
        if (url != null)
        {
            context.Request.Path = "/Catalogue";
            context.Request.QueryString = new QueryString(url);
            await _next.Invoke(context);
            return;
        }

        url = RewriteUtil.AnalysisLabel(context.Request.Path, settings);
        if (url != null)
        {
            context.Request.Path = "/Label";
            context.Request.QueryString = new QueryString(url);
            await _next.Invoke(context);
            return;
        }
        
        await _next.Invoke(context);
    }
    
}