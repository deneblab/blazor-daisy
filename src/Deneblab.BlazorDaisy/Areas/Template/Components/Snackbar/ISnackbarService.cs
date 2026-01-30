#pragma warning disable IDE0130
namespace Deneblab.BlazorDaisy.Components.Snackbar;

public interface ISnackbarService
{
    event Action<SnackbarMessage>? OnShow;
    event Action<Guid>? OnRemove;

    void Show(string message, Severity severity = Severity.Info, int durationMs = 3000);
    void Success(string message, int durationMs = 3000);
    void Error(string message, int durationMs = 5000);
    void Warning(string message, int durationMs = 4000);
    void Info(string message, int durationMs = 3000);
    void Remove(Guid id);
}
