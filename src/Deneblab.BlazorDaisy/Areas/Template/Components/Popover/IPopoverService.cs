#pragma warning disable IDE0130
using Microsoft.AspNetCore.Components;

namespace Deneblab.BlazorDaisy.Components.Popover;

public interface IPopoverService
{
    event Action<PopoverReference>? OnShow;
    event Action<Guid>? OnClose;

    PopoverReference Show(RenderFragment content, ElementReference anchorElement, PopoverOptions? options = null);
    void Close(PopoverReference popover);
    void Close(Guid popoverId);
    void CloseAll();
}
