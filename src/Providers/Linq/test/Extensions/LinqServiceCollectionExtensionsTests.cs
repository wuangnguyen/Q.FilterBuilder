using System;
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Core.TypeConversion;
using Q.FilterBuilder.Core.RuleTransformers;
using Q.FilterBuilder.Linq.Extensions;
using Xunit;

namespace Q.FilterBuilder.Linq.Tests.Extensions;

public class LinqServiceCollectionExtensionsTests
{
    [Fact]
    public void AddLinqFilterBuilder_ShouldRegisterFilterBuilder()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddLinqFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var filterBuilder = serviceProvider.GetService<IFilterBuilder>();
        Assert.NotNull(filterBuilder);
    }

    [Fact]
    public void AddLinqFilterBuilder_ShouldRegisterTypeConversionService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddLinqFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var typeConversionService = serviceProvider.GetService<ITypeConversionService>();
        Assert.NotNull(typeConversionService);
    }

    [Fact]
    public void AddLinqFilterBuilder_ShouldRegisterRuleTransformerService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddLinqFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var ruleTransformerService = serviceProvider.GetService<IRuleTransformerService>();
        Assert.NotNull(ruleTransformerService);
    }

    [Fact]
    public void AddLinqFilterBuilder_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        ServiceCollection? services = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => services!.AddLinqFilterBuilder());
        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void AddLinqFilterBuilder_ShouldGenerateLinqSpecificQuery()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLinqFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "equal", "John"));
        group.Rules.Add(new FilterRule("Age", "greater", 18));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.DoesNotContain("[", query); // LINQ doesn't use brackets for field names
        Assert.DoesNotContain("`", query); // LINQ doesn't use backticks for field names
        Assert.Contains("&&", query);      // LINQ uses && for AND
        Assert.Equal(2, parameters.Length);
        Assert.Equal("John", parameters[0]);
        Assert.Equal(18, parameters[1]);
    }

    [Fact]
    public void AddLinqFilterBuilder_ShouldSupportLinqSpecificOperators()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLinqFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "contains", "John"));
        group.Rules.Add(new FilterRule("CategoryId", "in", new[] { 1, 2, 3 }));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Contains("Contains", query);  // LINQ Contains method
        Assert.Contains("&&", query);        // LINQ AND operator
        Assert.Equal(2, parameters.Length);  // 1 for contains + 1 for in (collection)
    }
}
