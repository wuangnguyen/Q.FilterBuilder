using System;
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Core.TypeConversion;
using Q.FilterBuilder.Core.RuleTransformers;
using Q.FilterBuilder.SqlServer.Extensions;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests.Extensions;

public class SqlServerServiceCollectionExtensionsTests
{
    [Fact]
    public void AddSqlServerFilterBuilder_ShouldRegisterFilterBuilder()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var filterBuilder = serviceProvider.GetService<IFilterBuilder>();
        Assert.NotNull(filterBuilder);
    }

    [Fact]
    public void AddSqlServerFilterBuilder_ShouldRegisterTypeConversionService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var typeConversionService = serviceProvider.GetService<ITypeConversionService>();
        Assert.NotNull(typeConversionService);
    }

    [Fact]
    public void AddSqlServerFilterBuilder_ShouldRegisterRuleTransformerService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var ruleTransformerService = serviceProvider.GetService<IRuleTransformerService>();
        Assert.NotNull(ruleTransformerService);
    }

    [Fact]
    public void AddSqlServerFilterBuilder_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        ServiceCollection? services = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => services!.AddSqlServerFilterBuilder());
        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void AddSqlServerFilterBuilder_ShouldGenerateSqlServerSpecificQuery()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "equal", "John"));
        group.Rules.Add(new FilterRule("Age", "greater", 18));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Contains("[Name]", query); // SQL Server field formatting
        Assert.Contains("[Age]", query);  // SQL Server field formatting
        Assert.Contains("@p", query);     // SQL Server parameter formatting
        Assert.Equal(2, parameters.Length);
        Assert.Equal("John", parameters[0]);
        Assert.Equal(18, parameters[1]);
    }

    [Fact]
    public void AddSqlServerFilterBuilder_ShouldSupportSqlServerSpecificOperators()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "contains", "John"));
        group.Rules.Add(new FilterRule("CategoryId", "in", new[] { 1, 2, 3 }));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Contains("LIKE '%'", query);   // SQL Server LIKE syntax
        Assert.Contains("IN (", query);       // IN operator
        Assert.Equal(4, parameters.Length);   // 1 for contains + 3 for in
    }

    [Fact]
    public void AddSqlServerFilterBuilder_WithCustomTypeConversion_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var customConverterCalled = false;

        // Act - Explicitly cast to avoid ambiguity
        services.AddSqlServerFilterBuilder((Action<ITypeConversionService>)(typeConversion =>
        {
            customConverterCalled = true;
            // Custom type conversion configuration
        }));
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var filterBuilder = serviceProvider.GetService<IFilterBuilder>();
        var typeConversionService = serviceProvider.GetService<ITypeConversionService>();
        var ruleTransformerService = serviceProvider.GetService<IRuleTransformerService>();

        Assert.NotNull(filterBuilder);
        Assert.NotNull(typeConversionService);
        Assert.NotNull(ruleTransformerService);
        Assert.True(customConverterCalled);
    }

    [Fact]
    public void AddSqlServerFilterBuilder_WithCustomTypeConversion_ShouldSupportSqlServerOperators()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder((Action<ITypeConversionService>)(typeConversion =>
        {
            // Custom type conversion configuration
        }));
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "contains", "Test"));
        group.Rules.Add(new FilterRule("Id", "between", new[] { 1, 10 }));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Contains("LIKE '%'", query);     // SQL Server contains operator
        Assert.Contains("BETWEEN", query);      // SQL Server between operator
        Assert.Contains("[Name]", query);       // SQL Server field formatting
        Assert.Contains("@p", query);           // SQL Server parameter formatting
    }

    [Fact]
    public void AddSqlServerFilterBuilder_WithCustomRuleTransformers_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var customTransformerCalled = false;

        // Act - Explicitly cast to avoid ambiguity
        services.AddSqlServerFilterBuilder((Action<IRuleTransformerService>)(ruleTransformers =>
        {
            customTransformerCalled = true;
            // Custom rule transformer configuration
        }));
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var filterBuilder = serviceProvider.GetService<IFilterBuilder>();
        var typeConversionService = serviceProvider.GetService<ITypeConversionService>();
        var ruleTransformerService = serviceProvider.GetService<IRuleTransformerService>();

        Assert.NotNull(filterBuilder);
        Assert.NotNull(typeConversionService);
        Assert.NotNull(ruleTransformerService);
        Assert.True(customTransformerCalled);
    }

    [Fact]
    public void AddSqlServerFilterBuilder_WithCustomRuleTransformers_ShouldSupportSqlServerOperators()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder((Action<IRuleTransformerService>)(ruleTransformers =>
        {
            // Custom rule transformer configuration
        }));
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Status", "is_null", null));
        group.Rules.Add(new FilterRule("Tags", "in", new[] { "tag1", "tag2" }));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Contains("IS NULL", query);      // SQL Server is_null operator
        Assert.Contains("IN (", query);         // SQL Server in operator
        Assert.Contains("[Status]", query);     // SQL Server field formatting
        Assert.Contains("[Tags]", query);       // SQL Server field formatting
    }

    [Fact]
    public void AddSqlServerFilterBuilder_WithBothCustomConfigurations_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var typeConversionCalled = false;
        var ruleTransformerCalled = false;

        // Act - Use the 4-parameter overload to avoid ambiguity
        services.AddSqlServerFilterBuilder(
            typeConversion =>
            {
                typeConversionCalled = true;
                // Custom type conversion configuration
            },
            ruleTransformers =>
            {
                ruleTransformerCalled = true;
                // Custom rule transformer configuration
            });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var filterBuilder = serviceProvider.GetService<IFilterBuilder>();
        var typeConversionService = serviceProvider.GetService<ITypeConversionService>();
        var ruleTransformerService = serviceProvider.GetService<IRuleTransformerService>();

        Assert.NotNull(filterBuilder);
        Assert.NotNull(typeConversionService);
        Assert.NotNull(ruleTransformerService);
        Assert.True(typeConversionCalled);
        Assert.True(ruleTransformerCalled);
    }

    [Fact]
    public void AddSqlServerFilterBuilder_WithBothCustomConfigurations_ShouldSupportSqlServerOperators()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder(
            typeConversion =>
            {
                // Custom type conversion configuration
            },
            ruleTransformers =>
            {
                // Custom rule transformer configuration
            });
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Description", "contains", "test"));
        group.Rules.Add(new FilterRule("CreatedDate", "date_diff", "days"));
        group.Rules.Add(new FilterRule("IsActive", "is_not_null", null));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Contains("LIKE '%'", query);         // SQL Server contains operator
        Assert.Contains("DATEDIFF", query);         // SQL Server date_diff operator
        Assert.Contains("IS NOT NULL", query);      // SQL Server is_not_null operator
        Assert.Contains("[Description]", query);    // SQL Server field formatting
        Assert.Contains("[CreatedDate]", query);    // SQL Server field formatting
        Assert.Contains("[IsActive]", query);       // SQL Server field formatting
    }
}
