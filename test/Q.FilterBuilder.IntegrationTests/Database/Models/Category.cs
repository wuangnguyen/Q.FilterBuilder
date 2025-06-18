using System.ComponentModel.DataAnnotations;

namespace Q.FilterBuilder.IntegrationTests.Database.Models;

/// <summary>
/// Category entity for integration testing
/// </summary>
public class Category
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    // Navigation properties
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
