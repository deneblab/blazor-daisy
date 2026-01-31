#pragma warning disable IDE0130
using Microsoft.AspNetCore.Components;

namespace Deneblab.BlazorDaisy.Components.Dialog;

public interface IDialogService
{
    event Action<DialogReference>? OnShow;
    event Action<Guid, DialogResult>? OnClose;

    DialogReference Show<TDialog>(string? title = null, DialogParameters? parameters = null, DialogOptions? options = null)
        where TDialog : ComponentBase;

    DialogReference Show(Type dialogType, string? title = null, DialogParameters? parameters = null, DialogOptions? options = null);

    Task<DialogResult> ShowAsync<TDialog>(string? title = null, DialogParameters? parameters = null, DialogOptions? options = null)
        where TDialog : ComponentBase;

    Task<DialogResult> ShowAsync(Type dialogType, string? title = null, DialogParameters? parameters = null, DialogOptions? options = null);

    void Close(DialogReference dialog, DialogResult? result = null);
    void Close(Guid dialogId, DialogResult? result = null);
}
