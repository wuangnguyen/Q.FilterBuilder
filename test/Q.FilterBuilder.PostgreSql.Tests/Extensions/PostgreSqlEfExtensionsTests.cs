using System;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.PostgreSql.Extensions;
using Q.FilterBuilder.Tests.Shared;
using Xunit;

namespace Q.FilterBuilder.PostgreSql.Tests.Extensions;

public class PostgreSqlEfExtensionsTests
{
    [Fact]
    public void BuildForEf_ReplacesSingleParameterCorrectly()
    {
        // Arrange
        var builder = new TestFilterBuilder("\"Name\" = $1", ["John"]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForEf(group);

        // Assert
        Assert.Equal("\"Name\" = {0}", query);
        Assert.Single(parameters);
        Assert.Equal("John", parameters[0]);
    }

    [Fact]
    public void BuildForEf_ReplacesMultipleParametersCorrectly()
    {
        // Arrange
        var builder = new TestFilterBuilder("\"Name\" = $1 AND \"Age\" > $2", ["John", 30]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForEf(group);

        // Assert
        Assert.Equal("\"Name\" = {0} AND \"Age\" > {1}", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("John", parameters[0]);
        Assert.Equal(30, parameters[1]);
    }

    [Fact]
    public void BuildForEf_LeavesQueryUnchangedIfNoParameters()
    {
        // Arrange
        var builder = new TestFilterBuilder("\"IsActive\" = TRUE", Array.Empty<object>());
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForEf(group);

        // Assert
        Assert.Equal("\"IsActive\" = TRUE", query);
        Assert.Empty(parameters);
    }

    [Fact]
    public void BuildForEf_DoesNotReplaceSimilarButNonMatchingPatterns()
    {
        // Arrange
        var builder = new TestFilterBuilder("\"Name\" = $abc OR \"Other\" = $2", ["A", "B"]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForEf(group);

        // Assert
        Assert.Equal("\"Name\" = $abc OR \"Other\" = {1}", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("A", parameters[0]);
        Assert.Equal("B", parameters[1]);
    }

    [Fact]
    public void BuildForEf_ParametersArrayIsUnchanged()
    {
        // Arrange
        var originalParams = new object[] { 1, 2, 3 };
        var builder = new TestFilterBuilder("\"A\"=$1 OR \"B\"=$2 OR \"C\"=$3", originalParams);
        var group = new FilterGroup("OR");

        // Act
        var (_, parameters) = builder.BuildForEf(group);

        // Assert
        Assert.Same(originalParams, parameters);
    }

    [Fact]
    public void BuildForEf_HandlesNonSequentialParameterNumbers()
    {
        // Arrange
        var builder = new TestFilterBuilder("\"A\"=$2 OR \"B\"=$1", ["B", "A"]);
        var group = new FilterGroup("OR");

        // Act
        var (query, parameters) = builder.BuildForEf(group);

        // Assert
        Assert.Equal("\"A\"={1} OR \"B\"={0}", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("B", parameters[0]);
        Assert.Equal("A", parameters[1]);
    }
}
