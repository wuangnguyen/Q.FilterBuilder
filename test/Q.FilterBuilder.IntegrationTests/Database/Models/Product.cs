using System.ComponentModel.DataAnnotations;

namespace Q.FilterBuilder.IntegrationTests.Database.Models;

/// <summary>
/// Product entity for integration testing
/// </summary>
public class Product
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public bool IsAvailable { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }

    public double? Rating { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int? CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }

    // JSON-like properties for testing complex data types
    [MaxLength(2000)]
    public string? Tags { get; set; } // JSON array as string

    [MaxLength(2000)]
    public string? Metadata { get; set; } // JSON object as string
}
