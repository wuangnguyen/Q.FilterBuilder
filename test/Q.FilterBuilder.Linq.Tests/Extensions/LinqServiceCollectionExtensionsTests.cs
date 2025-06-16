using System;
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.Core.Providers;
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

    [Fact]
    public void AddLinqFilterBuilder_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddLinqFilterBuilder();

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddLinqFilterBuilder_WithOptions_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddLinqFilterBuilder(options => { });

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddLinqFilterBuilder_CalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert - Should not throw when called multiple times
        services.AddLinqFilterBuilder();
        services.AddLinqFilterBuilder();
        services.AddLinqFilterBuilder(options => { });

        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetService<IFilterBuilder>();
        Assert.NotNull(filterBuilder);
    }

    [Fact]
    public void AddLinqFilterBuilder_WithComplexOptions_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddLinqFilterBuilder(options => options
            .ConfigureTypeConversions(tc => { })
            .ConfigureRuleTransformers(rt => { }));

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var filterBuilder = serviceProvider.GetService<IFilterBuilder>();
        var typeConversionService = serviceProvider.GetService<ITypeConversionService>();
        var ruleTransformerService = serviceProvider.GetService<IRuleTransformerService>();

        Assert.NotNull(filterBuilder);
        Assert.NotNull(typeConversionService);
        Assert.NotNull(ruleTransformerService);
    }

    [Fact]
    public void AddLinqFilterBuilder_ShouldRegisterSingletonServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLinqFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var filterBuilder1 = serviceProvider.GetService<IFilterBuilder>();
        var filterBuilder2 = serviceProvider.GetService<IFilterBuilder>();

        // Assert - Should be same instance (singleton)
        Assert.Same(filterBuilder1, filterBuilder2);
    }

    [Fact]
    public void AddLinqFilterBuilder_ShouldRegisterLinqProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLinqFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var formatProvider = serviceProvider.GetService<IQueryFormatProvider>();

        // Assert
        Assert.NotNull(formatProvider);
        Assert.Equal("", formatProvider.ParameterPrefix);
        Assert.Equal("&&", formatProvider.AndOperator);
        Assert.Equal("||", formatProvider.OrOperator);
    }

    [Fact]
    public void AddLinqFilterBuilder_WithEmptyServiceCollection_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert - Should not throw with empty service collection
        services.AddLinqFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetService<IFilterBuilder>();

        Assert.NotNull(filterBuilder);
    }

    [Fact]
    public void AddLinqFilterBuilder_ShouldSupportQueryGeneration()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLinqFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "equal", "John"));
        group.Rules.Add(new FilterRule("Age", "between", new[] { 18, 65 }));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Contains("Name = @p0", query);
        Assert.Contains("Age >= @p1 && Age <= @p2", query);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("John", parameters[0]);
        Assert.Equal(18, parameters[1]);
        Assert.Equal(65, parameters[2]);
    }

    [Fact]
    public void AddLinqFilterBuilder_WithInvalidOptions_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert - Should throw ArgumentNullException for null configuration
        Assert.Throws<ArgumentNullException>(() =>
            services.AddLinqFilterBuilder(options => options
                .ConfigureTypeConversions(null!)));

        Assert.Throws<ArgumentNullException>(() =>
            services.AddLinqFilterBuilder(options => options
                .ConfigureRuleTransformers(null!)));
    }

    [Fact]
    public void AddLinqFilterBuilder_ShouldRegisterCoreServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLinqFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert - Verify core services are registered
        Assert.NotNull(serviceProvider.GetService<IFilterBuilder>());
        Assert.NotNull(serviceProvider.GetService<ITypeConversionService>());
        Assert.NotNull(serviceProvider.GetService<IRuleTransformerService>());
        Assert.NotNull(serviceProvider.GetService<IQueryFormatProvider>());
    }

    [Fact]
    public void AddLinqFilterBuilder_ShouldRegisterBasicOperators()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLinqFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var ruleTransformerService = serviceProvider.GetRequiredService<IRuleTransformerService>();

        // Act & Assert - Verify basic operators are registered
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("equal"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("not_equal"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("less"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("greater"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("less_or_equal"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("greater_or_equal"));
    }
}
