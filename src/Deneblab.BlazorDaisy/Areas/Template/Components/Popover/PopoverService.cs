using Microsoft.AspNetCore.Components;

namespace Deneblab.BlazorDaisy.Components.Popover;

public class PopoverService : IPopoverService
{
    private static event Action<PopoverReference>? OnShowStatic;
    private static event Action<Guid>? OnCloseStatic;

    public event Action<PopoverReference>? OnShow
    {
        add => OnShowStatic += value;
        remove => OnShowStatic -= value;
    }

    public event Action<Guid>? OnClose
    {
        add => OnCloseStatic += value;
        remove => OnCloseStatic -= value;
    }

    public PopoverReference Show(RenderFragment content, ElementReference anchorElement, PopoverOptions? options = null)
    {
        var reference = new PopoverReference(content, anchorElement, options);
        OnShowStatic?.Invoke(reference);
        return reference;
    }

    public void Close(PopoverReference popover)
    {
        Close(popover.Id);
    }

    public void Close(Guid popoverId)
    {
        OnCloseStatic?.Invoke(popoverId);
    }

    public void CloseAll()
    {
        // Provider will handle closing all
        OnCloseStatic?.Invoke(Guid.Empty);
    }
}
