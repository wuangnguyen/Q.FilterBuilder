using System;
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Core.TypeConversion;
using Q.FilterBuilder.Core.RuleTransformers;
using Q.FilterBuilder.PostgreSql.Extensions;
using Xunit;

namespace Q.FilterBuilder.PostgreSql.Tests.Extensions;

public class PostgreSqlServiceCollectionExtensionsTests
{
    [Fact]
    public void AddPostgreSqlFilterBuilder_ShouldRegisterFilterBuilder()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddPostgreSqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var filterBuilder = serviceProvider.GetService<IFilterBuilder>();
        Assert.NotNull(filterBuilder);
    }

    [Fact]
    public void AddPostgreSqlFilterBuilder_ShouldRegisterTypeConversionService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddPostgreSqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var typeConversionService = serviceProvider.GetService<ITypeConversionService>();
        Assert.NotNull(typeConversionService);
    }

    [Fact]
    public void AddPostgreSqlFilterBuilder_ShouldRegisterRuleTransformerService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddPostgreSqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var ruleTransformerService = serviceProvider.GetService<IRuleTransformerService>();
        Assert.NotNull(ruleTransformerService);
    }

    [Fact]
    public void AddPostgreSqlFilterBuilder_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        ServiceCollection? services = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => services!.AddPostgreSqlFilterBuilder());
        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void AddPostgreSqlFilterBuilder_ShouldGeneratePostgreSqlSpecificQuery()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddPostgreSqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "equal", "John"));
        group.Rules.Add(new FilterRule("Age", "greater", 18));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Contains("\"Name\"", query); // PostgreSQL field formatting
        Assert.Contains("\"Age\"", query);  // PostgreSQL field formatting
        Assert.Contains("$", query);       // PostgreSQL parameter formatting
        Assert.Equal(2, parameters.Length);
        Assert.Equal("John", parameters[0]);
        Assert.Equal(18, parameters[1]);
    }

    [Fact]
    public void AddPostgreSqlFilterBuilder_ShouldSupportBasicOperators()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddPostgreSqlFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "equal", "John"));
        group.Rules.Add(new FilterRule("Age", "greater", 25));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Contains("=", query);      // Equal operator
        Assert.Contains(">", query);      // Greater than operator
        Assert.Equal(2, parameters.Length); // 1 for equal + 1 for greater
    }
}
