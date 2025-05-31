using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.MySql.Extensions;
using Xunit;

namespace Q.FilterBuilder.MySql.Tests;

public class MySqlPerformanceTests
{
    [Fact]
    public void MySqlFilterBuilder_WithLargeNumberOfRules_ShouldPerformWell()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMySqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        
        // Add 1000 rules
        for (int i = 0; i < 1000; i++)
        {
            group.Rules.Add(new FilterRule($"Field{i}", "equal", $"Value{i}"));
        }

        // Act
        var stopwatch = Stopwatch.StartNew();
        var (query, parameters) = filterBuilder.Build(group);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(query);
        Assert.NotNull(parameters);
        Assert.Equal(1000, parameters.Length);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000, $"Performance test failed: took {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void MySqlFilterBuilder_WithDeeplyNestedGroups_ShouldPerformWell()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMySqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        // Create deeply nested structure (10 levels)
        var rootGroup = new FilterGroup("AND");
        var currentGroup = rootGroup;

        for (int i = 0; i < 10; i++)
        {
            currentGroup.Rules.Add(new FilterRule($"Field{i}", "equal", $"Value{i}"));
            
            var nestedGroup = new FilterGroup(i % 2 == 0 ? "AND" : "OR");
            currentGroup.Groups.Add(nestedGroup);
            currentGroup = nestedGroup;
        }

        // Act
        var stopwatch = Stopwatch.StartNew();
        var (query, parameters) = filterBuilder.Build(rootGroup);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(query);
        Assert.NotNull(parameters);
        Assert.Equal(10, parameters.Length);
        Assert.True(stopwatch.ElapsedMilliseconds < 100, $"Performance test failed: took {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void MySqlFilterBuilder_WithComplexOperators_ShouldPerformWell()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMySqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        
        // Add various complex operators
        for (int i = 0; i < 100; i++)
        {
            group.Rules.Add(new FilterRule($"Field{i}", "contains", $"Value{i}"));
            group.Rules.Add(new FilterRule($"Field{i}_between", "between", new[] { i, i + 100 }));
            group.Rules.Add(new FilterRule($"Field{i}_in", "in", new[] { $"A{i}", $"B{i}", $"C{i}" }));
        }

        // Act
        var stopwatch = Stopwatch.StartNew();
        var (query, parameters) = filterBuilder.Build(group);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(query);
        Assert.NotNull(parameters);
        Assert.Equal(600, parameters.Length); // 100 contains + 200 between + 300 in
        Assert.True(stopwatch.ElapsedMilliseconds < 500, $"Performance test failed: took {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void MySqlFilterBuilder_WithVeryLongFieldNames_ShouldPerformWell()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMySqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        
        // Add rules with very long field names
        for (int i = 0; i < 100; i++)
        {
            var longFieldName = new string('A', 1000) + i.ToString();
            group.Rules.Add(new FilterRule(longFieldName, "equal", $"Value{i}"));
        }

        // Act
        var stopwatch = Stopwatch.StartNew();
        var (query, parameters) = filterBuilder.Build(group);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(query);
        Assert.NotNull(parameters);
        Assert.Equal(100, parameters.Length);
        Assert.True(stopwatch.ElapsedMilliseconds < 200, $"Performance test failed: took {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void MySqlFilterBuilder_WithLargeArrayValues_ShouldPerformWell()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMySqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        
        // Create large arrays for IN operator
        var largeArray = new string[1000];
        for (int i = 0; i < 1000; i++)
        {
            largeArray[i] = $"Value{i}";
        }
        
        group.Rules.Add(new FilterRule("Status", "in", largeArray));
        group.Rules.Add(new FilterRule("Category", "in", largeArray));

        // Act
        var stopwatch = Stopwatch.StartNew();
        var (query, parameters) = filterBuilder.Build(group);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(query);
        Assert.NotNull(parameters);
        Assert.Equal(2000, parameters.Length);
        Assert.True(stopwatch.ElapsedMilliseconds < 300, $"Performance test failed: took {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void MySqlFilterBuilder_WithMultipleBuilds_ShouldBeCached()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMySqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "equal", "John"));
        group.Rules.Add(new FilterRule("Age", "greater", 25));

        // Act - Build multiple times
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 1000; i++)
        {
            var (query, parameters) = filterBuilder.Build(group);
        }
        stopwatch.Stop();

        // Assert
        Assert.True(stopwatch.ElapsedMilliseconds < 100, $"Caching test failed: took {stopwatch.ElapsedMilliseconds}ms for 1000 builds");
    }

    [Fact]
    public void MySqlFilterBuilder_WithConcurrentBuilds_ShouldBeThreadSafe()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMySqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "equal", "John"));

        // Act - Build concurrently
        var tasks = new Task[10];
        for (int i = 0; i < 10; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    var (query, parameters) = filterBuilder.Build(group);
                    Assert.NotNull(query);
                    Assert.NotNull(parameters);
                }
            });
        }

        // Assert
        Assert.True(Task.WaitAll(tasks, TimeSpan.FromSeconds(5)), "Concurrent builds should complete within 5 seconds");
    }

    [Fact]
    public void MySqlFilterBuilder_WithUnicodeContent_ShouldPerformWell()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMySqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        
        // Add rules with Unicode content
        for (int i = 0; i < 100; i++)
        {
            group.Rules.Add(new FilterRule($"用户名{i}", "equal", $"张三{i}"));
            group.Rules.Add(new FilterRule($"пользователь{i}", "contains", $"Иван{i}"));
            group.Rules.Add(new FilterRule($"🚀Field{i}", "begins_with", $"🌟Value{i}"));
        }

        // Act
        var stopwatch = Stopwatch.StartNew();
        var (query, parameters) = filterBuilder.Build(group);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(query);
        Assert.NotNull(parameters);
        Assert.Equal(300, parameters.Length);
        Assert.True(stopwatch.ElapsedMilliseconds < 200, $"Unicode performance test failed: took {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void MySqlFilterBuilder_MemoryUsage_ShouldBeReasonable()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMySqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        // Force garbage collection before test
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        
        var initialMemory = GC.GetTotalMemory(false);

        // Act - Create and build many filter groups
        for (int i = 0; i < 1000; i++)
        {
            var group = new FilterGroup("AND");
            group.Rules.Add(new FilterRule($"Field{i}", "equal", $"Value{i}"));
            group.Rules.Add(new FilterRule($"Field{i}_2", "contains", $"Search{i}"));
            
            var (query, parameters) = filterBuilder.Build(group);
        }

        var finalMemory = GC.GetTotalMemory(false);
        var memoryIncrease = finalMemory - initialMemory;

        // Assert - Memory increase should be reasonable (less than 10MB)
        Assert.True(memoryIncrease < 10 * 1024 * 1024, 
            $"Memory usage test failed: increased by {memoryIncrease / 1024 / 1024}MB");
    }
}
