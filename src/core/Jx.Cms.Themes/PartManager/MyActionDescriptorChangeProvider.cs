using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace Jx.Cms.Themes.PartManager;

public class MyActionDescriptorChangeProvider : IActionDescriptorChangeProvider
{
    private readonly object _syncRoot = new();
    public static MyActionDescriptorChangeProvider Instance { get; } = new();

    public CancellationTokenSource TokenSource { get; private set; } = new();

    public bool HasChanged { get; set; }

    public IChangeToken GetChangeToken()
    {
        return new CancellationChangeToken(TokenSource.Token);
    }

    public void NotifyChanges()
    {
        CancellationTokenSource previousTokenSource;
        lock (_syncRoot)
        {
            HasChanged = true;
            previousTokenSource = TokenSource;
            TokenSource = new CancellationTokenSource();
        }

        previousTokenSource.Cancel();
        previousTokenSource.Dispose();
    }
}
