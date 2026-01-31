using Microsoft.AspNetCore.Components;

namespace Deneblab.BlazorDaisy.Components.Dialog;

public class DialogService : IDialogService
{
    private static event Action<DialogReference>? OnShowStatic;
    private static event Action<Guid, DialogResult>? OnCloseStatic;

    public event Action<DialogReference>? OnShow
    {
        add => OnShowStatic += value;
        remove => OnShowStatic -= value;
    }

    public event Action<Guid, DialogResult>? OnClose
    {
        add => OnCloseStatic += value;
        remove => OnCloseStatic -= value;
    }

    public DialogReference Show<TDialog>(string? title = null, DialogParameters? parameters = null, DialogOptions? options = null)
        where TDialog : ComponentBase
    {
        return Show(typeof(TDialog), title, parameters, options);
    }

    public DialogReference Show(Type dialogType, string? title = null, DialogParameters? parameters = null, DialogOptions? options = null)
    {
        var reference = new DialogReference(dialogType, title, parameters, options);
        OnShowStatic?.Invoke(reference);
        return reference;
    }

    public Task<DialogResult> ShowAsync<TDialog>(string? title = null, DialogParameters? parameters = null, DialogOptions? options = null)
        where TDialog : ComponentBase
    {
        var reference = Show<TDialog>(title, parameters, options);
        return reference.Result;
    }

    public Task<DialogResult> ShowAsync(Type dialogType, string? title = null, DialogParameters? parameters = null, DialogOptions? options = null)
    {
        var reference = Show(dialogType, title, parameters, options);
        return reference.Result;
    }

    public void Close(DialogReference dialog, DialogResult? result = null)
    {
        Close(dialog.Id, result);
    }

    public void Close(Guid dialogId, DialogResult? result = null)
    {
        OnCloseStatic?.Invoke(dialogId, result ?? DialogResult.Cancel());
    }
}
