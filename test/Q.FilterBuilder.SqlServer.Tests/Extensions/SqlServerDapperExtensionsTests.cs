using Xunit;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.SqlServer.Extensions;
using System;
using Q.FilterBuilder.Tests.Shared;

namespace Q.FilterBuilder.SqlServer.Tests.Extensions;

public class SqlServerDapperExtensionsTests
{
    [Fact]
    public void BuildForDapper_CreatesParameterDictionary()
    {
        // Arrange
        var builder = new TestFilterBuilder("A = @p0 AND B = @p1", ["foo", 42]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForDapper(group);

        // Assert
        Assert.Equal("A = @p0 AND B = @p1", query);
        Assert.Equal(2, parameters.Count);
        Assert.Equal("foo", parameters["@p0"]);
        Assert.Equal(42, parameters["@p1"]);
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
    public void BuildForDapper_HandlesNullAndSpecialValues()
    {
        // Arrange
        var builder = new TestFilterBuilder("A = @p0 AND B = @p1", [null!, "@strange"]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForDapper(group);

        // Assert
        Assert.Equal("A = @p0 AND B = @p1", query);
        Assert.Null(parameters["@p0"]);
        Assert.Equal("@strange", parameters["@p1"]);
    }

    [Fact]
    public void BuildForDapper_HandlesMultipleParameters()
    {
        // Arrange
        var builder = new TestFilterBuilder("A = @p0 AND B = @p1 AND C = @p2", [1, 2, 3]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForDapper(group);

        // Assert
        Assert.Equal(3, parameters.Count);
        Assert.Equal(1, parameters["@p0"]);
        Assert.Equal(2, parameters["@p1"]);
        Assert.Equal(3, parameters["@p2"]);
    }

    [Fact]
    public void BuildForDapper_ParameterOrderIsPreserved()
    {
        // Arrange
        var builder = new TestFilterBuilder("A = @p0, B = @p1, C = @p2", ["first", "second", "third"]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForDapper(group);

        // Assert
        Assert.Equal("first", parameters["@p0"]);
        Assert.Equal("second", parameters["@p1"]);
        Assert.Equal("third", parameters["@p2"]);
    }

    [Fact]
    public void BuildForDapper_ThrowsIfParametersIsNull()
    {
        // Arrange
        var builder = new TestFilterBuilder("A = @p0", null!);
        var group = new FilterGroup("AND");

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => builder.BuildForDapper(group));
    }
}
