using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Linq.Extensions;
using Xunit;

namespace Q.FilterBuilder.Linq.Tests;

public class LinqPerformanceTests
{
    private readonly IFilterBuilder _filterBuilder;

    public LinqPerformanceTests()
    {
        var services = new ServiceCollection();
        services.AddLinqFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        _filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();
    }

    [Fact]
    public void Build_WithLargeNumberOfRules_ShouldCompleteInReasonableTime()
    {
        // Arrange
        var group = new FilterGroup("AND");
        for (int i = 0; i < 500; i++)
        {
            group.Rules.Add(new FilterRule($"Field{i}", "equal", $"Value{i}"));
        }

        // Act
        var stopwatch = Stopwatch.StartNew();
        var (query, parameters) = _filterBuilder.Build(group);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(query);
        Assert.Equal(500, parameters.Length);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000, $"Query building took {stopwatch.ElapsedMilliseconds}ms, expected < 1000ms");
    }

    [Fact]
    public void Build_WithDeeplyNestedGroups_ShouldCompleteInReasonableTime()
    {
        // Arrange
        var rootGroup = new FilterGroup("AND");
        var currentGroup = rootGroup;

        // Create 25 levels of nesting
        for (int i = 0; i < 25; i++)
        {
            currentGroup.Rules.Add(new FilterRule($"Field{i}", "equal", $"Value{i}"));
            
            var nestedGroup = new FilterGroup(i % 2 == 0 ? "AND" : "OR");
            currentGroup.Groups.Add(nestedGroup);
            currentGroup = nestedGroup;
        }

        // Add final rule
        currentGroup.Rules.Add(new FilterRule("FinalField", "equal", "FinalValue"));

        // Act
        var stopwatch = Stopwatch.StartNew();
        var (query, parameters) = _filterBuilder.Build(rootGroup);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(query);
        Assert.Equal(26, parameters.Length);
        Assert.True(stopwatch.ElapsedMilliseconds < 500, $"Query building took {stopwatch.ElapsedMilliseconds}ms, expected < 500ms");
    }

    [Fact]
    public void Build_WithManyStringOperations_ShouldCompleteInReasonableTime()
    {
        // Arrange
        var group = new FilterGroup("OR");
        var operators = new[] { "contains", "begins_with", "ends_with", "not_contains" };
        
        for (int i = 0; i < 200; i++)
        {
            var operatorName = operators[i % operators.Length];
            group.Rules.Add(new FilterRule($"TextField{i}", operatorName, $"SearchValue{i}"));
        }

        // Act
        var stopwatch = Stopwatch.StartNew();
        var (query, parameters) = _filterBuilder.Build(group);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(query);
        Assert.Equal(200, parameters.Length);
        Assert.True(stopwatch.ElapsedMilliseconds < 800, $"Query building took {stopwatch.ElapsedMilliseconds}ms, expected < 800ms");
    }

    [Fact]
    public void Build_WithManyInOperators_ShouldCompleteInReasonableTime()
    {
        // Arrange
        var group = new FilterGroup("AND");
        
        for (int i = 0; i < 50; i++)
        {
            var values = new[] { i * 10, i * 10 + 1, i * 10 + 2 };
            group.Rules.Add(new FilterRule($"ListField{i}", "in", values));
        }

        // Act
        var stopwatch = Stopwatch.StartNew();
        var (query, parameters) = _filterBuilder.Build(group);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(query);
        Assert.Equal(50, parameters.Length);
        Assert.True(stopwatch.ElapsedMilliseconds < 300, $"Query building took {stopwatch.ElapsedMilliseconds}ms, expected < 300ms");
    }

    [Fact]
    public void Build_WithComplexMixedOperators_ShouldCompleteInReasonableTime()
    {
        // Arrange
        var group = new FilterGroup("AND");
        
        // Add various types of operations
        for (int i = 0; i < 25; i++)
        {
            // Basic comparison
            group.Rules.Add(new FilterRule($"NumField{i}", "greater", i));
            
            // String operations
            group.Rules.Add(new FilterRule($"StrField{i}", "contains", $"text{i}"));
            
            // In operations
            group.Rules.Add(new FilterRule($"ListField{i}", "in", new[] { i, i + 1, i + 2 }));
            
            // Between operations
            group.Rules.Add(new FilterRule($"RangeField{i}", "between", new[] { i, i + 10 }));
        }

        // Act
        var stopwatch = Stopwatch.StartNew();
        var (query, parameters) = _filterBuilder.Build(group);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(query);
        Assert.True(parameters.Length > 0);
        Assert.True(stopwatch.ElapsedMilliseconds < 600, $"Query building took {stopwatch.ElapsedMilliseconds}ms, expected < 600ms");
    }

    [Fact]
    public void Build_RepeatedCalls_ShouldMaintainPerformance()
    {
        // Arrange
        var group = new FilterGroup("AND");
        for (int i = 0; i < 50; i++)
        {
            group.Rules.Add(new FilterRule($"Field{i}", "equal", $"Value{i}"));
        }

        var times = new System.Collections.Generic.List<long>();

        // Act - Build the same query multiple times
        for (int iteration = 0; iteration < 10; iteration++)
        {
            var stopwatch = Stopwatch.StartNew();
            var (query, parameters) = _filterBuilder.Build(group);
            stopwatch.Stop();
            times.Add(stopwatch.ElapsedMilliseconds);

            Assert.NotNull(query);
            Assert.Equal(50, parameters.Length);
        }

        // Assert - Performance should be consistent
        var maxTime = Math.Max(times[0], Math.Max(times[1], times[2]));
        var minTime = Math.Min(times[0], Math.Min(times[1], times[2]));
        
        Assert.True(maxTime < 100, $"Max time was {maxTime}ms, expected < 100ms");
        Assert.True(maxTime - minTime < 50, $"Performance variance too high: {maxTime - minTime}ms");
    }

    [Fact]
    public void Build_WithLargeStringValues_ShouldCompleteInReasonableTime()
    {
        // Arrange
        var group = new FilterGroup("OR");
        var largeString = new string('A', 5000); // 5KB string
        
        for (int i = 0; i < 25; i++)
        {
            group.Rules.Add(new FilterRule($"LargeField{i}", "contains", largeString + i));
        }

        // Act
        var stopwatch = Stopwatch.StartNew();
        var (query, parameters) = _filterBuilder.Build(group);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(query);
        Assert.Equal(25, parameters.Length);
        Assert.True(stopwatch.ElapsedMilliseconds < 200, $"Query building took {stopwatch.ElapsedMilliseconds}ms, expected < 200ms");
    }

    [Fact]
    public void Build_MemoryUsage_ShouldBeReasonable()
    {
        // Arrange
        var group = new FilterGroup("AND");
        for (int i = 0; i < 500; i++)
        {
            group.Rules.Add(new FilterRule($"Field{i}", "equal", $"Value{i}"));
        }

        // Act
        var initialMemory = GC.GetTotalMemory(true);
        var (query, parameters) = _filterBuilder.Build(group);
        var finalMemory = GC.GetTotalMemory(false);

        // Assert
        Assert.NotNull(query);
        Assert.Equal(500, parameters.Length);
        
        var memoryUsed = finalMemory - initialMemory;
        Assert.True(memoryUsed < 5_000_000, $"Memory usage was {memoryUsed} bytes, expected < 5MB");
    }

    [Fact]
    public void Build_ConcurrentAccess_ShouldBeThreadSafe()
    {
        // Arrange
        var group = new FilterGroup("AND");
        for (int i = 0; i < 50; i++)
        {
            group.Rules.Add(new FilterRule($"Field{i}", "equal", $"Value{i}"));
        }

        var exceptions = new System.Collections.Generic.List<Exception>();
        var results = new System.Collections.Generic.List<(string query, object[] parameters)>();
        var lockObject = new object();

        // Act - Run multiple threads concurrently
        var tasks = new System.Collections.Generic.List<System.Threading.Tasks.Task>();
        for (int t = 0; t < 5; t++)
        {
            tasks.Add(System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    for (int i = 0; i < 5; i++)
                    {
                        var result = _filterBuilder.Build(group);
                        lock (lockObject)
                        {
                            results.Add(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    lock (lockObject)
                    {
                        exceptions.Add(ex);
                    }
                }
            }));
        }

        System.Threading.Tasks.Task.WaitAll(tasks.ToArray());

        // Assert
        Assert.Empty(exceptions);
        Assert.Equal(25, results.Count);
        
        // All results should be identical
        var firstResult = results[0];
        foreach (var result in results)
        {
            Assert.Equal(firstResult.query, result.query);
            Assert.Equal(firstResult.parameters.Length, result.parameters.Length);
        }
    }
}
