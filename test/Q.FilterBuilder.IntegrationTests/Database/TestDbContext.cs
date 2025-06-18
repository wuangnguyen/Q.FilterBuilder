using Microsoft.EntityFrameworkCore;
using Q.FilterBuilder.IntegrationTests.Database.Models;

namespace Q.FilterBuilder.IntegrationTests.Database;

/// <summary>
/// Entity Framework DbContext for integration testing
/// </summary>
public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Salary).HasPrecision(18, 2);
            entity.Property(e => e.Department).HasMaxLength(50);
            entity.Property(e => e.Role).HasMaxLength(50);
            
            // Relationships
            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Users)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Tags).HasMaxLength(2000);
            entity.Property(e => e.Metadata).HasMaxLength(2000);
            
            // Relationships
            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.CreatedByUser)
                  .WithMany(u => u.Products)
                  .HasForeignKey(e => e.CreatedByUserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Technology", Description = "Technology products and services", IsActive = true, CreatedDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 2, Name = "Marketing", Description = "Marketing and sales", IsActive = true, CreatedDate = new DateTime(2023, 1, 2, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 3, Name = "Finance", Description = "Financial services", IsActive = false, CreatedDate = new DateTime(2023, 1, 3, 0, 0, 0, DateTimeKind.Utc) }
        );

        // Seed Users
        modelBuilder.Entity<User>().HasData(
            new User 
            { 
                Id = 1, 
                Name = "John Doe", 
                Email = "john.doe@company.com", 
                Age = 30, 
                Salary = 75000.00m, 
                IsActive = true, 
                CreatedDate = new DateTime(2023, 1, 15, 0, 0, 0, DateTimeKind.Utc), 
                LastLoginDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                Department = "Technology",
                Role = "Developer",
                CategoryId = 1
            },
            new User 
            { 
                Id = 2, 
                Name = "Jane Smith", 
                Email = "jane.smith@company.com", 
                Age = 28, 
                Salary = 65000.00m, 
                IsActive = true, 
                CreatedDate = new DateTime(2023, 2, 1, 0, 0, 0, DateTimeKind.Utc), 
                LastLoginDate = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                Department = "Marketing",
                Role = "Manager",
                CategoryId = 2
            },
            new User 
            { 
                Id = 3, 
                Name = "Bob Johnson", 
                Email = "bob.johnson@company.com", 
                Age = 35, 
                Salary = 85000.00m, 
                IsActive = false, 
                CreatedDate = new DateTime(2023, 3, 1, 0, 0, 0, DateTimeKind.Utc), 
                LastLoginDate = null,
                Department = "Finance",
                Role = "Analyst",
                CategoryId = 3
            },
            new User 
            { 
                Id = 4, 
                Name = "Alice Brown", 
                Email = "alice.brown@company.com", 
                Age = 32, 
                Salary = 70000.00m, 
                IsActive = true, 
                CreatedDate = new DateTime(2023, 4, 1, 0, 0, 0, DateTimeKind.Utc), 
                LastLoginDate = new DateTime(2024, 1, 12, 0, 0, 0, DateTimeKind.Utc),
                Department = "Technology",
                Role = "Senior Developer",
                CategoryId = 1
            }
        );

        // Seed Products
        modelBuilder.Entity<Product>().HasData(
            new Product 
            { 
                Id = 1, 
                Name = "Laptop Pro", 
                Description = "High-performance laptop for developers", 
                Price = 1299.99m, 
                Stock = 50, 
                IsAvailable = true, 
                CreatedDate = new DateTime(2023, 5, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedDate = new DateTime(2023, 12, 1, 0, 0, 0, DateTimeKind.Utc),
                Status = "Active",
                Rating = 4.5,
                CategoryId = 1,
                CreatedByUserId = 1,
                Tags = "[\"laptop\", \"technology\", \"development\"]",
                Metadata = "{\"brand\": \"TechCorp\", \"warranty\": \"2 years\", \"features\": [\"SSD\", \"16GB RAM\"]}"
            },
            new Product 
            { 
                Id = 2, 
                Name = "Marketing Suite", 
                Description = "Complete marketing automation platform", 
                Price = 299.99m, 
                Stock = 100, 
                IsAvailable = true, 
                CreatedDate = new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedDate = new DateTime(2023, 11, 15, 0, 0, 0, DateTimeKind.Utc),
                Status = "Active",
                Rating = 4.2,
                CategoryId = 2,
                CreatedByUserId = 2,
                Tags = "[\"marketing\", \"automation\", \"analytics\"]",
                Metadata = "{\"type\": \"SaaS\", \"users\": \"unlimited\", \"integrations\": [\"email\", \"social\"]}"
            },
            new Product 
            { 
                Id = 3, 
                Name = "Financial Dashboard", 
                Description = "Real-time financial reporting dashboard", 
                Price = 599.99m, 
                Stock = 25, 
                IsAvailable = false, 
                CreatedDate = new DateTime(2023, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedDate = null,
                Status = "Discontinued",
                Rating = 3.8,
                CategoryId = 3,
                CreatedByUserId = 3,
                Tags = "[\"finance\", \"reporting\", \"dashboard\"]",
                Metadata = "{\"compliance\": \"SOX\", \"currencies\": [\"USD\", \"EUR\", \"GBP\"]}"
            },
            new Product 
            { 
                Id = 4, 
                Name = "Code Editor Pro", 
                Description = "Advanced code editor with AI assistance", 
                Price = 99.99m, 
                Stock = 200, 
                IsAvailable = true, 
                CreatedDate = new DateTime(2023, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Status = "Active",
                Rating = 4.8,
                CategoryId = 1,
                CreatedByUserId = 4,
                Tags = "[\"editor\", \"AI\", \"development\", \"productivity\"]",
                Metadata = "{\"languages\": [\"C#\", \"JavaScript\", \"Python\"], \"ai_features\": true}"
            }
        );
    }
}
