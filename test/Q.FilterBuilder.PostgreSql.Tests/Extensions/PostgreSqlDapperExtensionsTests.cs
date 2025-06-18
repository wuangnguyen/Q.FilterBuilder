using Xunit;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.PostgreSql.Extensions;
using System;
using Q.FilterBuilder.Tests.Shared;

namespace Q.FilterBuilder.PostgreSql.Tests.Extensions;

public class PostgreSqlDapperExtensionsTests
{
    [Fact]
    public void BuildForDapper_ConvertsNumberedParameters()
    {
        // Arrange
        var builder = new TestFilterBuilder("Column1 = $1 AND Column2 = $2", ["value1", 123]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForDapper(group);

        // Assert
        Assert.Equal("Column1 = @p0 AND Column2 = @p1", query);
        Assert.Equal(2, parameters.Count);
        Assert.Equal("value1", parameters["@p0"]);
        Assert.Equal(123, parameters["@p1"]);
    }

    [Fact]
    public void BuildForDapper_HandlesEmptyParameterArray()
    {
        // Arrange
        var builder = new TestFilterBuilder("NoParamsQuery", Array.Empty<object>());
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForDapper(group);

        // Assert
        Assert.Equal("NoParamsQuery", query);
        Assert.Empty(parameters);
    }

    [Fact]
    public void BuildForDapper_ReplacesAllOccurrencesOfSameParameter()
    {
        // Arrange
        var builder = new TestFilterBuilder("A = $1 OR B = $1", [42]);
        var group = new FilterGroup("OR");

        // Act
        var (query, parameters) = builder.BuildForDapper(group);

        // Assert
        Assert.Equal("A = @p0 OR B = @p0", query);
        Assert.Single(parameters);
        Assert.Equal(42, parameters["@p0"]);
    }

    [Fact]
    public void BuildForDapper_HandlesNonSequentialPlaceholders()
    {
        // Arrange
        var builder = new TestFilterBuilder("A = $1 AND B = $3", ["x", "y", "z"]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForDapper(group);

        // Assert
        Assert.Equal("A = @p0 AND B = @p2", query);
        Assert.Equal(3, parameters.Count);
        Assert.Equal("x", parameters["@p0"]);
        Assert.Equal("y", parameters["@p1"]);
        Assert.Equal("z", parameters["@p2"]);
    }

    [Fact]
    public void BuildForDapper_HandlesNullAndSpecialValues()
    {
        // Arrange
        var builder = new TestFilterBuilder("A = $1 AND B = $2", [null!, "@weird$"]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForDapper(group);

        // Assert
        Assert.Equal("A = @p0 AND B = @p1", query);
        Assert.Null(parameters["@p0"]);
        Assert.Equal("@weird$", parameters["@p1"]);
    }

    [Fact]
    public void BuildForDapper_ThrowsIfBuilderReturnsNullParameters()
    {
        // Arrange
        var builder = new TestFilterBuilder("A = $1", null!);
        var group = new FilterGroup("AND");

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => builder.BuildForDapper(group));
    }

    [Fact]
    public void BuildForDapper_HandlesQueryWithNoPlaceholders()
    {
        // Arrange
        var builder = new TestFilterBuilder("SELECT 1", ["unused"]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForDapper(group);

        // Assert
        Assert.Equal("SELECT 1", query);
        Assert.Single(parameters);
        Assert.Equal("unused", parameters["@p0"]);
    }
}
