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

    [Fact]
    public void AddLinqFilterBuilder_WithOptions_ShouldRegisterFilterBuilder()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddLinqFilterBuilder(options => options
            .ConfigureTypeConversions(typeConversion =>
            {
                // Custom type conversion configuration
            })
            .ConfigureRuleTransformers(ruleTransformers =>
            {
                // Custom rule transformer configuration
            }));
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var filterBuilder = serviceProvider.GetService<IFilterBuilder>();
        Assert.NotNull(filterBuilder);
    }

    [Fact]
    public void AddLinqFilterBuilder_WithNullOptions_ShouldRegisterFilterBuilder()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddLinqFilterBuilder(null);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var filterBuilder = serviceProvider.GetService<IFilterBuilder>();
        Assert.NotNull(filterBuilder);
    }

    [Fact]
    public void AddLinqFilterBuilder_ShouldRegisterLinqSpecificTransformers()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLinqFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var ruleTransformerService = serviceProvider.GetRequiredService<IRuleTransformerService>();

        // Act & Assert - Test that LINQ-specific transformers are registered
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("between"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("not_between"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("in"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("not_in"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("contains"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("contains_any"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("not_contains"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("begins_with"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("not_begins_with"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("ends_with"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("not_ends_with"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("is_null"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("is_not_null"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("is_empty"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("is_not_empty"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("date_diff"));
    }

    [Fact]
    public void AddLinqFilterBuilder_ShouldGenerateLinqSpecificSyntax()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLinqFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "contains", "John"));
        group.Rules.Add(new FilterRule("Status", "in", new[] { "Active", "Pending" }));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Contains("Contains", query);    // LINQ Contains method
        Assert.Contains("@", query);           // LINQ parameter syntax
        Assert.Equal(2, parameters.Length);   // 1 for contains + 1 for in
    }
}
