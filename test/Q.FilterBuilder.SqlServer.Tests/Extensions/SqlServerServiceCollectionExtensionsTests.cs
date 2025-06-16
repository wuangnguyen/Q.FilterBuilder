using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.Core.TypeConversion;
using Q.FilterBuilder.Core.RuleTransformers;
using Q.FilterBuilder.Core.Providers;
using Q.FilterBuilder.SqlServer.Extensions;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests.Extensions;

public class SqlServerServiceCollectionExtensionsTests
{
    [Fact]
    public void AddSqlServerFilterBuilder_WithoutOptions_ShouldRegisterAllServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSqlServerFilterBuilder();
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
        Assert.Contains("@p0", query);     // SQL Server parameter formatting
        Assert.Equal(2, parameters.Length);
        Assert.Equal("John", parameters[0]);
        Assert.Equal(18, parameters[1]);
    }

    [Fact]
    public void AddSqlServerFilterBuilder_ShouldSupportCoreOperators()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var ruleTransformerService = serviceProvider.GetRequiredService<IRuleTransformerService>();

        // Act & Assert - Core operators should be available
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("equal"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("not_equal"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("less"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("less_or_equal"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("greater"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("greater_or_equal"));
    }

    [Fact]
    public void AddSqlServerFilterBuilder_ShouldSupportSqlServerSpecificOperators()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var ruleTransformerService = serviceProvider.GetRequiredService<IRuleTransformerService>();

        // Act & Assert - SQL Server specific operators should be available
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
    public void AddSqlServerFilterBuilder_ShouldGenerateCorrectSqlServerQueries()
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

        // Act
        services.AddSqlServerFilterBuilder(options => options
            .ConfigureTypeConversion(typeConversion =>
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
    public void AddSqlServerFilterBuilder_WithCustomRuleTransformers_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var customTransformerCalled = false;

        // Act
        services.AddSqlServerFilterBuilder(options => options
            .ConfigureRuleTransformers(ruleTransformers =>
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
    public void AddSqlServerFilterBuilder_WithBothCustomConfigurations_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var typeConversionCalled = false;
        var ruleTransformerCalled = false;

        // Act
        services.AddSqlServerFilterBuilder(options => options
            .ConfigureTypeConversion(typeConversion =>
            {
                typeConversionCalled = true;
                // Custom type conversion configuration
            })
            .ConfigureRuleTransformers(ruleTransformers =>
            {
                ruleTransformerCalled = true;
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
        Assert.True(typeConversionCalled);
        Assert.True(ruleTransformerCalled);
    }

    [Fact]
    public void AddSqlServerFilterBuilder_WithMultipleCustomTypeConversions_ShouldRegisterAll()
    {
        // Arrange
        var services = new ServiceCollection();
        var converter1Called = false;
        var converter2Called = false;
        var converter3Called = false;

        // Act
        services.AddSqlServerFilterBuilder(options => options
            .ConfigureTypeConversion(typeConversion =>
            {
                typeConversion.RegisterConverter("custom1", new TestTypeConverter(() => converter1Called = true));
                typeConversion.RegisterConverter("custom2", new TestTypeConverter(() => converter2Called = true));
                typeConversion.RegisterConverter("custom3", new TestTypeConverter(() => converter3Called = true));
            }));
        var serviceProvider = services.BuildServiceProvider();
        var typeConversionService = serviceProvider.GetRequiredService<ITypeConversionService>();

        // Assert - Test that converters work by calling ConvertValue
        var result1 = typeConversionService.ConvertValue("test1", "custom1");
        var result2 = typeConversionService.ConvertValue("test2", "custom2");
        var result3 = typeConversionService.ConvertValue("test3", "custom3");

        Assert.Equal("test1", result1);
        Assert.Equal("test2", result2);
        Assert.Equal("test3", result3);
        Assert.True(converter1Called);
        Assert.True(converter2Called);
        Assert.True(converter3Called);
    }

    [Fact]
    public void AddSqlServerFilterBuilder_WithMultipleCustomRuleTransformers_ShouldRegisterAll()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSqlServerFilterBuilder(options => options
            .ConfigureRuleTransformers(ruleTransformers =>
            {
                ruleTransformers.RegisterTransformer("custom_search", new TestRuleTransformer("CUSTOM_SEARCH"));
                ruleTransformers.RegisterTransformer("fuzzy_match", new TestRuleTransformer("FUZZY_MATCH"));
                ruleTransformers.RegisterTransformer("geo_distance", new TestRuleTransformer("GEO_DISTANCE"));
            }));
        var serviceProvider = services.BuildServiceProvider();
        var ruleTransformerService = serviceProvider.GetRequiredService<IRuleTransformerService>();

        // Assert
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("custom_search"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("fuzzy_match"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("geo_distance"));

        // Should still have SQL Server transformers
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("contains"));
        Assert.NotNull(ruleTransformerService.GetRuleTransformer("between"));
    }

    [Fact]
    public void AddSqlServerFilterBuilder_WithComplexConfiguration_ShouldSupportAllFeatures()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder(options => options
            .ConfigureTypeConversion(tc =>
            {
                tc.RegisterConverter("custom_date", new TestTypeConverter(() => { }));
            })
            .ConfigureRuleTransformers(rt =>
            {
                rt.RegisterTransformer("custom_op", new TestRuleTransformer("CUSTOM"));
            }));
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "contains", "test"));
        group.Rules.Add(new FilterRule("Id", "between", new[] { 1, 10 }));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Contains("LIKE '%'", query);     // SQL Server contains operator
        Assert.Contains("BETWEEN", query);      // SQL Server between operator
        Assert.Contains("[Name]", query);       // SQL Server field formatting
        Assert.Contains("@p0", query);           // SQL Server parameter formatting
    }

    [Fact]
    public void AddSqlServerFilterBuilder_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddSqlServerFilterBuilder();

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddSqlServerFilterBuilder_ShouldRegisterSqlServerFormatProvider()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var formatProvider = serviceProvider.GetService<IQueryFormatProvider>();
        Assert.NotNull(formatProvider);
        Assert.IsType<SqlServerFormatProvider>(formatProvider);
    }

    [Fact]
    public void AddSqlServerFilterBuilder_WithExceptionInTypeConversionConfiguration_ShouldPropagateException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSqlServerFilterBuilder(options => options
            .ConfigureTypeConversion(tc =>
            {
                throw new InvalidOperationException("Test exception in type conversion");
            }));
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.Throws<InvalidOperationException>(() =>
            serviceProvider.GetRequiredService<IFilterBuilder>());
    }

    [Fact]
    public void AddSqlServerFilterBuilder_WithExceptionInRuleTransformerConfiguration_ShouldPropagateException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSqlServerFilterBuilder(options => options
            .ConfigureRuleTransformers(rt =>
            {
                throw new InvalidOperationException("Test exception in rule transformer");
            }));
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.Throws<InvalidOperationException>(() =>
            serviceProvider.GetRequiredService<IFilterBuilder>());
    }

    [Fact]
    public void AddSqlServerFilterBuilder_WithMultipleRegistrations_ShouldAllowMultiple()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSqlServerFilterBuilder();
        services.AddSqlServerFilterBuilder(); // Second registration

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilders = serviceProvider.GetServices<IFilterBuilder>().ToList();
        Assert.Equal(2, filterBuilders.Count); // Should have multiple registrations since AddSingleton allows duplicates
    }

    [Fact]
    public void AddSqlServerFilterBuilder_WithNullOrEmptyConfiguration_ShouldWork()
    {
        // Arrange
        var services1 = new ServiceCollection();
        var services2 = new ServiceCollection();

        // Act
        services1.AddSqlServerFilterBuilder(null);
        services2.AddSqlServerFilterBuilder(options => { });

        var serviceProvider1 = services1.BuildServiceProvider();
        var serviceProvider2 = services2.BuildServiceProvider();

        // Assert
        var filterBuilder1 = serviceProvider1.GetService<IFilterBuilder>();
        var filterBuilder2 = serviceProvider2.GetService<IFilterBuilder>();
        Assert.NotNull(filterBuilder1);
        Assert.NotNull(filterBuilder2);
    }

    [Fact]
    public void AddSqlServerFilterBuilder_ConfigurationShouldBeCalledOnce()
    {
        // Arrange
        var services = new ServiceCollection();
        var configurationCallCount = 0;

        // Act
        services.AddSqlServerFilterBuilder(options =>
        {
            configurationCallCount++;
            options.ConfigureTypeConversion(tc => { });
        });
        var serviceProvider = services.BuildServiceProvider();

        // Get the service multiple times
        var filterBuilder1 = serviceProvider.GetRequiredService<IFilterBuilder>();
        var filterBuilder2 = serviceProvider.GetRequiredService<IFilterBuilder>();

        // Assert
        Assert.Equal(1, configurationCallCount);
        Assert.Same(filterBuilder1, filterBuilder2);
    }

    // Test helper classes
    private class TestTypeConverter : ITypeConverter<object>
    {
        private readonly Action _onConvert;

        public TestTypeConverter(Action onConvert)
        {
            _onConvert = onConvert;
        }

        public object Convert(object? value, Dictionary<string, object?>? metadata = null)
        {
            _onConvert();
            return value ?? new object();
        }
    }

    private class TestRuleTransformer : IRuleTransformer
    {
        private readonly string _operation;

        public TestRuleTransformer(string operation)
        {
            _operation = operation;
        }

        public (string query, object[]? parameters) Transform(FilterRule rule, string fieldName, int parameterIndex, IQueryFormatProvider formatProvider)
        {
            var parameterName = formatProvider.FormatParameterName(parameterIndex);
            return ($"{fieldName} {_operation} {parameterName}", rule.Value != null ? new[] { rule.Value } : null);
        }
    }
}
