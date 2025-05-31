using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Core;
using Q.FilterBuilder.SqlServer.Extensions;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests;

public class SqlServerPerformanceTests
{
    [Fact]
    public void SqlServerFilterBuilder_WithLargeNumberOfRules_ShouldPerformWell()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        
        // Add 1000 rules to test performance
        for (int i = 0; i < 1000; i++)
        {
            group.Rules.Add(new FilterRule($"Field{i}", "equal", $"Value{i}"));
        }

        var stopwatch = Stopwatch.StartNew();

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        stopwatch.Stop();
        
        Assert.NotNull(query);
        Assert.NotNull(parameters);
        Assert.Equal(1000, parameters.Length);
        
        // Performance assertion - should complete within reasonable time
        Assert.True(stopwatch.ElapsedMilliseconds < 1000, 
            $"Building query with 1000 rules took {stopwatch.ElapsedMilliseconds}ms, expected < 1000ms");
    }

    [Fact]
    public void SqlServerFilterBuilder_WithDeeplyNestedGroups_ShouldPerformWell()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        // Create deeply nested groups (10 levels deep)
        var rootGroup = new FilterGroup("AND");
        var currentGroup = rootGroup;

        for (int i = 0; i < 10; i++)
        {
            currentGroup.Rules.Add(new FilterRule($"Field{i}", "equal", $"Value{i}"));
            
            var nestedGroup = new FilterGroup(i % 2 == 0 ? "AND" : "OR");
            currentGroup.Groups.Add(nestedGroup);
            currentGroup = nestedGroup;
        }

        // Add final rule to the deepest group
        currentGroup.Rules.Add(new FilterRule("FinalField", "equal", "FinalValue"));

        var stopwatch = Stopwatch.StartNew();

        // Act
        var (query, parameters) = filterBuilder.Build(rootGroup);

        // Assert
        stopwatch.Stop();
        
        Assert.NotNull(query);
        Assert.NotNull(parameters);
        Assert.Equal(11, parameters.Length); // 10 + 1 final rule
        
        // Performance assertion
        Assert.True(stopwatch.ElapsedMilliseconds < 100, 
            $"Building deeply nested query took {stopwatch.ElapsedMilliseconds}ms, expected < 100ms");
    }

    [Fact]
    public void SqlServerFilterBuilder_WithComplexOperators_ShouldPerformWell()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        
        // Add various complex operators
        for (int i = 0; i < 100; i++)
        {
            switch (i % 8)
            {
                case 0:
                    group.Rules.Add(new FilterRule($"Field{i}", "between", new[] { i, i + 10 }));
                    break;
                case 1:
                    group.Rules.Add(new FilterRule($"Field{i}", "in", new[] { $"Value{i}", $"Value{i+1}", $"Value{i+2}" }));
                    break;
                case 2:
                    group.Rules.Add(new FilterRule($"Field{i}", "contains", $"Value{i}"));
                    break;
                case 3:
                    group.Rules.Add(new FilterRule($"Field{i}", "begins_with", $"Value{i}"));
                    break;
                case 4:
                    group.Rules.Add(new FilterRule($"Field{i}", "ends_with", $"Value{i}"));
                    break;
                case 5:
                    group.Rules.Add(new FilterRule($"Field{i}", "is_null", null));
                    break;
                case 6:
                    group.Rules.Add(new FilterRule($"Field{i}", "is_empty", null));
                    break;
                case 7:
                    var rule = new FilterRule($"Field{i}", "date_diff", 30);
                    rule.Metadata = new Dictionary<string, object?> { { "intervalType", "day" } };
                    group.Rules.Add(rule);
                    break;
            }
        }

        var stopwatch = Stopwatch.StartNew();

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        stopwatch.Stop();
        
        Assert.NotNull(query);
        
        // Performance assertion
        Assert.True(stopwatch.ElapsedMilliseconds < 500, 
            $"Building complex query took {stopwatch.ElapsedMilliseconds}ms, expected < 500ms");
    }

    [Fact]
    public void SqlServerFilterBuilder_WithVeryLongFieldNames_ShouldPerformWell()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        
        // Add rules with very long field names
        for (int i = 0; i < 100; i++)
        {
            var longFieldName = new string('A', 1000) + i.ToString(); // 1000+ character field name
            group.Rules.Add(new FilterRule(longFieldName, "equal", $"Value{i}"));
        }

        var stopwatch = Stopwatch.StartNew();

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        stopwatch.Stop();
        
        Assert.NotNull(query);
        Assert.NotNull(parameters);
        Assert.Equal(100, parameters.Length);
        
        // Performance assertion
        Assert.True(stopwatch.ElapsedMilliseconds < 200, 
            $"Building query with long field names took {stopwatch.ElapsedMilliseconds}ms, expected < 200ms");
    }

    [Fact]
    public void SqlServerFilterBuilder_WithLargeArrayValues_ShouldPerformWell()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        
        // Create large arrays for IN operators
        for (int i = 0; i < 10; i++)
        {
            var largeArray = new string[1000];
            for (int j = 0; j < 1000; j++)
            {
                largeArray[j] = $"Value{i}_{j}";
            }
            group.Rules.Add(new FilterRule($"Field{i}", "in", largeArray));
        }

        var stopwatch = Stopwatch.StartNew();

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        stopwatch.Stop();
        
        Assert.NotNull(query);
        Assert.NotNull(parameters);
        Assert.Equal(10000, parameters.Length); // 10 fields * 1000 values each
        
        // Performance assertion
        Assert.True(stopwatch.ElapsedMilliseconds < 1000, 
            $"Building query with large arrays took {stopwatch.ElapsedMilliseconds}ms, expected < 1000ms");
    }

    [Fact]
    public void SqlServerFilterBuilder_MultipleBuilds_ShouldBeCached()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "equal", "John"));
        group.Rules.Add(new FilterRule("Age", "greater", 25));

        // First build to warm up
        filterBuilder.Build(group);

        var stopwatch = Stopwatch.StartNew();

        // Act - Build the same query multiple times
        for (int i = 0; i < 1000; i++)
        {
            var (query, parameters) = filterBuilder.Build(group);
        }

        // Assert
        stopwatch.Stop();
        
        // Performance assertion - subsequent builds should be fast
        Assert.True(stopwatch.ElapsedMilliseconds < 100, 
            $"Building same query 1000 times took {stopwatch.ElapsedMilliseconds}ms, expected < 100ms");
    }

    [Fact]
    public void SqlServerFilterBuilder_ConcurrentBuilds_ShouldBeThreadSafe()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "equal", "John"));

        var exceptions = new List<Exception>();
        var tasks = new List<System.Threading.Tasks.Task>();

        // Act - Run multiple builds concurrently
        for (int i = 0; i < 10; i++)
        {
            var task = System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    for (int j = 0; j < 100; j++)
                    {
                        var (query, parameters) = filterBuilder.Build(group);
                        Assert.NotNull(query);
                        Assert.NotNull(parameters);
                    }
                }
                catch (Exception ex)
                {
                    lock (exceptions)
                    {
                        exceptions.Add(ex);
                    }
                }
            });
            tasks.Add(task);
        }

        System.Threading.Tasks.Task.WaitAll(tasks.ToArray());

        // Assert
        Assert.Empty(exceptions);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithUnicodeContent_ShouldPerformWell()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        
        // Add rules with Unicode content
        var unicodeStrings = new[]
        {
            "用户名", "пользователь", "ユーザー", "사용자", "المستخدم", "उपयोगकर्ता",
            "🚀🌟💫", "Ñoño", "Café", "naïve", "résumé", "Москва"
        };

        for (int i = 0; i < 100; i++)
        {
            var unicodeValue = unicodeStrings[i % unicodeStrings.Length] + i.ToString();
            group.Rules.Add(new FilterRule($"Field{i}", "contains", unicodeValue));
        }

        var stopwatch = Stopwatch.StartNew();

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        stopwatch.Stop();
        
        Assert.NotNull(query);
        Assert.NotNull(parameters);
        Assert.Equal(100, parameters.Length);
        
        // Performance assertion
        Assert.True(stopwatch.ElapsedMilliseconds < 200, 
            $"Building query with Unicode content took {stopwatch.ElapsedMilliseconds}ms, expected < 200ms");
    }

    [Fact]
    public void SqlServerFilterBuilder_MemoryUsage_ShouldBeReasonable()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        // Force garbage collection before test
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var initialMemory = GC.GetTotalMemory(false);

        // Act - Build many queries
        for (int i = 0; i < 1000; i++)
        {
            var group = new FilterGroup("AND");
            group.Rules.Add(new FilterRule($"Field{i}", "equal", $"Value{i}"));
            group.Rules.Add(new FilterRule($"Field{i}_2", "contains", $"Value{i}_2"));
            
            var (query, parameters) = filterBuilder.Build(group);
        }

        var finalMemory = GC.GetTotalMemory(false);
        var memoryIncrease = finalMemory - initialMemory;

        // Assert - Memory increase should be reasonable (less than 10MB for 1000 queries)
        Assert.True(memoryIncrease < 10 * 1024 * 1024, 
            $"Memory increased by {memoryIncrease / 1024 / 1024}MB, expected < 10MB");
    }
}
