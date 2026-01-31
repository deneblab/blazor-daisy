#pragma warning disable IDE0130
namespace Deneblab.BlazorDaisy.Components.Popover;

public class PopoverOptions
{
    public Origin AnchorOrigin { get; set; } = Origin.BottomLeft;
    public Origin TransformOrigin { get; set; } = Origin.TopLeft;
    public bool Fixed { get; set; }
    public bool CloseOnClickOutside { get; set; } = true;
    public string? Class { get; set; }
    public string? Style { get; set; }
}
