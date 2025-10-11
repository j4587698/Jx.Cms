using Microsoft.AspNetCore.Components;

namespace Jx.Cms.Install.Components;

public class StepBase : ComponentBase
{
    [Parameter] public Action Prev { get; set; }

    [Parameter] public Action Next { get; set; }
}