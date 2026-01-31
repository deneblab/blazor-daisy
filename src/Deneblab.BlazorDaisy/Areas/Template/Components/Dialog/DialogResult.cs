namespace Deneblab.BlazorDaisy.Components.Dialog;

public class DialogResult
{
    public bool Canceled { get; }
    public object? Data { get; }

    protected DialogResult(bool canceled, object? data)
    {
        Canceled = canceled;
        Data = data;
    }

    public static DialogResult Ok() => new(false, null);
    public static DialogResult Ok<T>(T data) => new(false, data);
    public static DialogResult Cancel() => new(true, null);

    public T? GetData<T>() => Data is T data ? data : default;
}
