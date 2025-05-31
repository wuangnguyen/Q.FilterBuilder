using System;
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Core.Providers;
using Q.FilterBuilder.Core.TypeConversion;
using Q.FilterBuilder.Core.RuleTransformers;
using Q.FilterBuilder.MySql.Extensions;
using Xunit;

namespace Q.FilterBuilder.MySql.Tests.Extensions;

public class MySqlServiceCollectionExtensionsTests
{
    [Fact]
    public void AddMySqlFilterBuilder_ShouldRegisterAllServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMySqlFilterBuilder();
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
    public void AddMySqlFilterBuilder_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        ServiceCollection? services = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => services!.AddMySqlFilterBuilder());
        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void AddMySqlFilterBuilder_ShouldGenerateMySqlSpecificQuery()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMySqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "equal", "John"));
        group.Rules.Add(new FilterRule("Age", "greater", 18));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Contains("`Name`", query); // MySQL field formatting
        Assert.Contains("`Age`", query);  // MySQL field formatting
        Assert.Contains("?", query);      // MySQL parameter formatting
        Assert.Equal(2, parameters.Length);
        Assert.Equal("John", parameters[0]);
        Assert.Equal(18, parameters[1]);
    }

    [Fact]
    public void AddMySqlFilterBuilder_WithNullOrCustomOptions_ShouldRegisterFilterBuilder()
    {
        // Arrange
        var services1 = new ServiceCollection();
        var services2 = new ServiceCollection();

        // Act
        services1.AddMySqlFilterBuilder(null);
        services2.AddMySqlFilterBuilder(options => options
            .ConfigureTypeConversions(typeConversion =>
            {
                // Custom type conversion configuration
            })
            .ConfigureRuleTransformers(ruleTransformers =>
            {
                // Custom rule transformer configuration
            }));

        var serviceProvider1 = services1.BuildServiceProvider();
        var serviceProvider2 = services2.BuildServiceProvider();

        // Assert
        var filterBuilder1 = serviceProvider1.GetService<IFilterBuilder>();
        var filterBuilder2 = serviceProvider2.GetService<IFilterBuilder>();
        Assert.NotNull(filterBuilder1);
        Assert.NotNull(filterBuilder2);
    }

    [Fact]
    public void AddMySqlFilterBuilder_ShouldRegisterMySqlSpecificTransformers()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMySqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var ruleTransformerService = serviceProvider.GetRequiredService<IRuleTransformerService>();

        // Act & Assert - Test that MySQL-specific transformers are registered
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("between"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("not_between"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("in"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("not_in"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("contains"));
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
    public void AddMySqlFilterBuilder_ShouldGenerateMySqlSpecificSyntax()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMySqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "contains", "John"));
        group.Rules.Add(new FilterRule("Status", "in", new[] { "Active", "Pending" }));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Contains("LIKE CONCAT", query); // MySQL concatenation syntax
        Assert.Contains("IN (", query);        // IN operator
        Assert.Contains("?", query);           // MySQL parameter syntax
        Assert.Equal(3, parameters.Length);   // 1 for contains + 2 for in
    }

    [Fact]
    public void AddMySqlFilterBuilder_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddMySqlFilterBuilder();

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddMySqlFilterBuilder_WithOptions_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddMySqlFilterBuilder(options => { });

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddMySqlFilterBuilder_CalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert - Should not throw when called multiple times
        services.AddMySqlFilterBuilder();
        services.AddMySqlFilterBuilder();
        services.AddMySqlFilterBuilder(options => { });

        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetService<IFilterBuilder>();
        Assert.NotNull(filterBuilder);
    }

    [Fact]
    public void AddMySqlFilterBuilder_WithComplexOptions_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMySqlFilterBuilder(options => options
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
    public void AddMySqlFilterBuilder_ShouldRegisterSingletonServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMySqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var filterBuilder1 = serviceProvider.GetService<IFilterBuilder>();
        var filterBuilder2 = serviceProvider.GetService<IFilterBuilder>();

        // Assert - Should be same instance (singleton)
        Assert.Same(filterBuilder1, filterBuilder2);
    }

    [Fact]
    public void AddMySqlFilterBuilder_ShouldRegisterMySqlProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMySqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var formatProvider = serviceProvider.GetService<IQueryFormatProvider>();

        // Assert
        Assert.NotNull(formatProvider);
        Assert.Equal("?", formatProvider.ParameterPrefix);
        Assert.Equal("AND", formatProvider.AndOperator);
        Assert.Equal("OR", formatProvider.OrOperator);
    }

    [Fact]
    public void AddMySqlFilterBuilder_WithEmptyServiceCollection_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert - Should not throw with empty service collection
        services.AddMySqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetService<IFilterBuilder>();

        Assert.NotNull(filterBuilder);
    }

    [Fact]
    public void AddMySqlFilterBuilder_ShouldSupportQueryGeneration()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMySqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "equal", "John"));
        group.Rules.Add(new FilterRule("Age", "between", new[] { 18, 65 }));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Contains("`Name` = ?", query);
        Assert.Contains("`Age` BETWEEN ? AND ?", query);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("John", parameters[0]);
        Assert.Equal(18, parameters[1]);
        Assert.Equal(65, parameters[2]);
    }

    [Fact]
    public void AddMySqlFilterBuilder_WithInvalidOptions_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert - Should throw ArgumentNullException for null configuration
        Assert.Throws<ArgumentNullException>(() =>
            services.AddMySqlFilterBuilder(options => options
                .ConfigureTypeConversions(null!)));

        Assert.Throws<ArgumentNullException>(() =>
            services.AddMySqlFilterBuilder(options => options
                .ConfigureRuleTransformers(null!)));
    }

    [Fact]
    public void AddMySqlFilterBuilder_ShouldRegisterCoreServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMySqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert - Verify core services are registered
        Assert.NotNull(serviceProvider.GetService<IFilterBuilder>());
        Assert.NotNull(serviceProvider.GetService<ITypeConversionService>());
        Assert.NotNull(serviceProvider.GetService<IRuleTransformerService>());
        Assert.NotNull(serviceProvider.GetService<IQueryFormatProvider>());
    }

    [Fact]
    public void AddMySqlFilterBuilder_ShouldRegisterBasicOperators()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMySqlFilterBuilder();
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
