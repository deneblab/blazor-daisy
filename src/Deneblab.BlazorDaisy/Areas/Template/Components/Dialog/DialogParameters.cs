using Microsoft.AspNetCore.Components;

namespace Deneblab.BlazorDaisy.Components.Dialog;

public class DialogParameters : Dictionary<string, object?>
{
    public DialogParameters() : base(StringComparer.OrdinalIgnoreCase) { }

    public void Add<T>(string key, T? value)
    {
        this[key] = value;
    }

    public T? Get<T>(string key)
    {
        if (TryGetValue(key, out var value) && value is T typed)
        {
            return typed;
        }
        return default;
    }
}

public class DialogParameters<TDialog> : DialogParameters where TDialog : ComponentBase
{
    public void Add<TValue>(System.Linq.Expressions.Expression<Func<TDialog, TValue>> property, TValue? value)
    {
        if (property.Body is System.Linq.Expressions.MemberExpression member)
        {
            this[member.Member.Name] = value;
        }
    }
}
