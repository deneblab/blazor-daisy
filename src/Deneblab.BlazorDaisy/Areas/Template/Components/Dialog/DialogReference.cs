namespace Deneblab.BlazorDaisy.Components.Dialog;

public class DialogReference
{
    public Guid Id { get; } = Guid.NewGuid();
    public Type DialogType { get; }
    public string? Title { get; }
    public DialogParameters Parameters { get; }
    public DialogOptions Options { get; }

    private readonly TaskCompletionSource<DialogResult> _resultSource = new();

    public DialogReference(Type dialogType, string? title, DialogParameters? parameters, DialogOptions? options)
    {
        DialogType = dialogType;
        Title = title;
        Parameters = parameters ?? new DialogParameters();
        Options = options ?? new DialogOptions();
    }

    public Task<DialogResult> Result => _resultSource.Task;

    internal void Close(DialogResult result)
    {
        _resultSource.TrySetResult(result);
    }
}
