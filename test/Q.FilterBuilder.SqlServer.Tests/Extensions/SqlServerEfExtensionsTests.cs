using System;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.SqlServer.Extensions;
using Q.FilterBuilder.Tests.Shared;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests.Extensions;

public class SqlServerEfExtensionsTests
{
    [Fact]
    public void BuildForEf_ReplacesSingleParameterCorrectly()
    {
        // Arrange
        var builder = new TestFilterBuilder("[Name] = @p0", ["John"]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForEf(group);

        // Assert
        Assert.Equal("[Name] = {0}", query);
        Assert.Single(parameters);
        Assert.Equal("John", parameters[0]);
    }

    [Fact]
    public void BuildForEf_ReplacesMultipleParametersCorrectly()
    {
        // Arrange
        var builder = new TestFilterBuilder("[Name] = @p0 AND [Age] > @p1", ["John", 30]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForEf(group);

        // Assert
        Assert.Equal("[Name] = {0} AND [Age] > {1}", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("John", parameters[0]);
        Assert.Equal(30, parameters[1]);
    }

    [Fact]
    public void BuildForEf_LeavesQueryUnchangedIfNoParameters()
    {
        // Arrange
        var builder = new TestFilterBuilder("[IsActive] = 1", Array.Empty<object>());
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForEf(group);

        // Assert
        Assert.Equal("[IsActive] = 1", query);
        Assert.Empty(parameters);
    }

    [Fact]
    public void BuildForEf_DoesNotReplaceSimilarButNonMatchingPatterns()
    {
        // Arrange
        var builder = new TestFilterBuilder("[Name] = @param0 OR [Other] = @p1", ["A", "B"]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForEf(group);

        // Assert
        Assert.Equal("[Name] = @param0 OR [Other] = {1}", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("A", parameters[0]);
        Assert.Equal("B", parameters[1]);
    }

    [Fact]
    public void BuildForEf_ParametersArrayIsUnchanged()
    {
        // Arrange
        var originalParams = new object[] { 1, 2, 3 };
        var builder = new TestFilterBuilder("[A]=@p0 OR [B]=@p1 OR [C]=@p2", originalParams);

        // Act
        var group = new FilterGroup("OR");

        // Assert
        var (_, parameters) = builder.BuildForEf(group);
        Assert.Same(originalParams, parameters);
    }
}
