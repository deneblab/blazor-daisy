using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Deneblab.BlazorDaisy.Areas.Example;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ExampleController : ControllerBase
{
    // Example in-memory data store (replace with your database)
    private static readonly List<ExampleItem> _items = new()
    {
        new ExampleItem { Id = 1, Name = "Item 1", Description = "First example item" },
        new ExampleItem { Id = 2, Name = "Item 2", Description = "Second example item" }
    };

    private readonly ILogger<ExampleController> _logger;

    public ExampleController(ILogger<ExampleController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    ///     GET /api/example - Get all items
    /// </summary>
    [HttpGet]
    public ActionResult<List<ExampleItem>> GetAll()
    {
        _logger.LogInformation("Getting all example items");
        return Ok(_items);
    }

    /// <summary>
    ///     GET /api/example/{id} - Get item by ID
    /// </summary>
    [HttpGet("{id:int}")]
    public ActionResult<ExampleItem> GetById(int id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        if (item == null) return NotFound(new { error = $"Item with ID {id} not found" });
        return Ok(item);
    }

    /// <summary>
    ///     POST /api/example - Create new item
    /// </summary>
    [HttpPost]
    public ActionResult<ExampleItem> Create([FromBody] CreateExampleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name)) return BadRequest(new { error = "Name is required" });

        var newItem = new ExampleItem
        {
            Id = _items.Count > 0 ? _items.Max(x => x.Id) + 1 : 1,
            Name = request.Name,
            Description = request.Description
        };

        _items.Add(newItem);
        _logger.LogInformation("Created example item with ID {Id}", newItem.Id);

        return CreatedAtAction(nameof(GetById), new { id = newItem.Id }, newItem);
    }

    /// <summary>
    ///     PUT /api/example/{id} - Update existing item
    /// </summary>
    [HttpPut("{id:int}")]
    public ActionResult<ExampleItem> Update(int id, [FromBody] UpdateExampleRequest request)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        if (item == null) return NotFound(new { error = $"Item with ID {id} not found" });

        if (!string.IsNullOrWhiteSpace(request.Name)) item.Name = request.Name;

        if (request.Description != null) item.Description = request.Description;

        _logger.LogInformation("Updated example item with ID {Id}", id);
        return Ok(item);
    }

    /// <summary>
    ///     DELETE /api/example/{id} - Delete item
    /// </summary>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        if (item == null) return NotFound(new { error = $"Item with ID {id} not found" });

        _items.Remove(item);
        _logger.LogInformation("Deleted example item with ID {Id}", id);

        return NoContent();
    }

    /// <summary>
    ///     GET /api/example/protected - Example of protected endpoint (requires authentication)
    /// </summary>
    [HttpGet("protected")]
    [Authorize]
    public ActionResult<object> ProtectedEndpoint()
    {
        var userName = User.Identity?.Name ?? "Unknown";
        return Ok(new { message = $"Hello, {userName}! This is a protected endpoint." });
    }
}

// TEMPLATE: Example models - replace with your own domain models

public class ExampleItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateExampleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateExampleRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}