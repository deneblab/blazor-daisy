namespace Deneblab.BlazorDaisy.Services.Navigation;

/// <summary>
/// Service for managing dynamic navigation menu items.
/// </summary>
public interface IMenuService
{
    /// <summary>
    /// Gets all root menu items (items without a parent).
    /// </summary>
    IReadOnlyList<MenuItem> GetMenuItems();

    /// <summary>
    /// Gets menu items for a specific area.
    /// </summary>
    IReadOnlyList<MenuItem> GetMenuItemsByArea(string area);

    /// <summary>
    /// Registers a new menu item.
    /// </summary>
    void Register(MenuItem item);

    /// <summary>
    /// Registers multiple menu items at once.
    /// </summary>
    void RegisterRange(IEnumerable<MenuItem> items);

    /// <summary>
    /// Removes a menu item by its ID.
    /// </summary>
    bool Remove(string id);

    /// <summary>
    /// Clears all menu items.
    /// </summary>
    void Clear();

    /// <summary>
    /// Event raised when menu items change.
    /// </summary>
    event Action? OnMenuChanged;
}
