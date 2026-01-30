namespace Deneblab.BlazorDaisy.Components.Snackbar;

public class SnackbarService : ISnackbarService
{
    // Static events allow communication across different Blazor circuits
    // (e.g., between SSR pages and interactive components)
    private static event Action<SnackbarMessage>? OnShowStatic;
    private static event Action<Guid>? OnRemoveStatic;

    public event Action<SnackbarMessage>? OnShow
    {
        add => OnShowStatic += value;
        remove => OnShowStatic -= value;
    }

    public event Action<Guid>? OnRemove
    {
        add => OnRemoveStatic += value;
        remove => OnRemoveStatic -= value;
    }

    public void Show(string message, Severity severity = Severity.Info, int durationMs = 3000)
    {
        var snackbar = new SnackbarMessage
        {
            Message = message,
            Severity = severity,
            DurationMs = durationMs
        };

        OnShowStatic?.Invoke(snackbar);
    }

    public void Success(string message, int durationMs = 3000)
        => Show(message, Severity.Success, durationMs);

    public void Error(string message, int durationMs = 5000)
        => Show(message, Severity.Error, durationMs);

    public void Warning(string message, int durationMs = 4000)
        => Show(message, Severity.Warning, durationMs);

    public void Info(string message, int durationMs = 3000)
        => Show(message, Severity.Info, durationMs);

    public void Remove(Guid id)
        => OnRemoveStatic?.Invoke(id);
}
