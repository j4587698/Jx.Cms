using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.Logging;

namespace Jx.Cms.Themes.RazorCompiler;

public class MyViewCompilerProvider : IViewCompilerProvider
{
    private readonly ApplicationPartManager _applicationPartManager;
    private readonly ILoggerFactory _loggerFactory;
    private MyViewCompiler _compiler;

    public MyViewCompilerProvider(
        ApplicationPartManager applicationPartManager,
        ILoggerFactory loggerFactory)
    {
        _applicationPartManager = applicationPartManager;
        _loggerFactory = loggerFactory;
        Modify();
    }

    public IViewCompiler GetCompiler()
    {
        return _compiler;
    }

    public void Modify()
    {
        var feature = new ViewsFeature();
        _applicationPartManager.PopulateFeature(feature);

        _compiler = new MyViewCompiler(feature.ViewDescriptors, _loggerFactory.CreateLogger<MyViewCompiler>());
    }
}