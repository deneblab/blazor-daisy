namespace Deneblab.BlazorDaisy.Components.Snackbar;

public class SnackbarMessage
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Message { get; init; } = string.Empty;
    public Severity Severity { get; init; } = Severity.Info;
    public int DurationMs { get; init; } = 3000;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}
