using System.Threading;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace Jx.Cms.Themes.PartManager
{
    public class MyActionDescriptorChangeProvider: IActionDescriptorChangeProvider
    {
        public static MyActionDescriptorChangeProvider Instance { get; } = new MyActionDescriptorChangeProvider();

        public CancellationTokenSource TokenSource { get; private set; }

        public bool HasChanged { get; set; }

        public IChangeToken GetChangeToken()
        {
            TokenSource = new CancellationTokenSource();
            return new CancellationChangeToken(TokenSource.Token);
        }
    }
}