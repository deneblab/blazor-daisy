using Microsoft.AspNetCore.Components;

namespace Deneblab.BlazorDaisy.Components.Popover;

public class PopoverReference
{
    public Guid Id { get; } = Guid.NewGuid();
    public RenderFragment Content { get; }
    public ElementReference AnchorElement { get; }
    public PopoverOptions Options { get; }
    public bool IsOpen { get; internal set; } = true;

    public PopoverReference(RenderFragment content, ElementReference anchorElement, PopoverOptions? options = null)
    {
        Content = content;
        AnchorElement = anchorElement;
        Options = options ?? new PopoverOptions();
    }
}
