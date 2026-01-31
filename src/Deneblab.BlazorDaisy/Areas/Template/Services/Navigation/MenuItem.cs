using System;
using System.Collections.Generic;
#pragma warning disable IDE0130

namespace Deneblab.BlazorDaisy.Navigation;

/// <summary>
/// Represents a menu item in the navigation sidebar.
/// </summary>
public class MenuItem
{
    /// <summary>
    /// Unique identifier for the menu item.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Display title of the menu item.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Navigation href/URL for the menu item.
    /// </summary>
    public required string Href { get; set; }

    /// <summary>
    /// Icon name or SVG path for the menu item.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Display order (lower numbers appear first).
    /// </summary>
    public int Order { get; set; } = 100;

    /// <summary>
    /// Parent menu item ID for nested menus (null for root items).
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// Child menu items for submenus.
    /// </summary>
    public List<MenuItem> Children { get; set; } = [];

    /// <summary>
    /// Whether this menu item requires authentication.
    /// </summary>
    public bool RequiresAuth { get; set; } = false;

    /// <summary>
    /// Required roles to see this menu item (empty = all authenticated users).
    /// </summary>
    public List<string> RequiredRoles { get; set; } = [];

    /// <summary>
    /// Area name this menu item belongs to (for organization).
    /// </summary>
    public string? Area { get; set; }
}
