namespace Deneblab.BlazorDaisy.Services.Navigation;

/// <summary>
/// Default implementation of the menu service.
/// Manages dynamic menu registration for the navigation sidebar.
/// </summary>
public class MenuService : IMenuService
{
    private readonly List<MenuItem> _items = [];
    private readonly object _lock = new();

    public event Action? OnMenuChanged;

    public IReadOnlyList<MenuItem> GetMenuItems()
    {
        lock (_lock)
        {
            return BuildMenuTree(_items.Where(i => i.ParentId == null));
        }
    }

    public IReadOnlyList<MenuItem> GetMenuItemsByArea(string area)
    {
        lock (_lock)
        {
            var areaItems = _items.Where(i =>
                string.Equals(i.Area, area, StringComparison.OrdinalIgnoreCase) &&
                i.ParentId == null);
            return BuildMenuTree(areaItems);
        }
    }

    public void Register(MenuItem item)
    {
        lock (_lock)
        {
            // Remove existing item with same ID if present
            _items.RemoveAll(i => i.Id == item.Id);
            _items.Add(item);
        }
        OnMenuChanged?.Invoke();
    }

    public void RegisterRange(IEnumerable<MenuItem> items)
    {
        lock (_lock)
        {
            foreach (var item in items)
            {
                _items.RemoveAll(i => i.Id == item.Id);
                _items.Add(item);
            }
        }
        OnMenuChanged?.Invoke();
    }

    public bool Remove(string id)
    {
        bool removed;
        lock (_lock)
        {
            removed = _items.RemoveAll(i => i.Id == id) > 0;
        }
        if (removed)
        {
            OnMenuChanged?.Invoke();
        }
        return removed;
    }

    public void Clear()
    {
        lock (_lock)
        {
            _items.Clear();
        }
        OnMenuChanged?.Invoke();
    }

    private List<MenuItem> BuildMenuTree(IEnumerable<MenuItem> rootItems)
    {
        var result = rootItems.OrderBy(i => i.Order).ThenBy(i => i.Title).ToList();

        foreach (var item in result)
        {
            item.Children = _items
                .Where(i => i.ParentId == item.Id)
                .OrderBy(i => i.Order)
                .ThenBy(i => i.Title)
                .ToList();
        }

        return result;
    }
}
