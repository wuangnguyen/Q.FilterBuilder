using System.ComponentModel.DataAnnotations;

namespace Q.FilterBuilder.IntegrationTests.Database.Models;

/// <summary>
/// User entity for integration testing
/// </summary>
public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    public int Age { get; set; }

    public decimal Salary { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? LastLoginDate { get; set; }

    [MaxLength(50)]
    public string? Department { get; set; }

    [MaxLength(50)]
    public string? Role { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    // Navigation property
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
