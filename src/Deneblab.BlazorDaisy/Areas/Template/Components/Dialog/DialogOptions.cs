#pragma warning disable IDE0130
namespace Deneblab.BlazorDaisy.Components.Dialog;

public class DialogOptions
{
    public string? MaxWidth { get; set; }
    public bool FullWidth { get; set; }
    public bool CloseOnEscapeKey { get; set; } = true;
    public bool CloseOnBackdropClick { get; set; } = true;
    public bool ShowCloseButton { get; set; } = true;
    public DialogPosition Position { get; set; } = DialogPosition.Center;
    public string? BackdropClass { get; set; }
    public string? DialogClass { get; set; }
}

public enum DialogPosition
{
    Center,
    Top,
    Bottom
}
