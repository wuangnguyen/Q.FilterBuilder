using System;
using Xunit;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.MySql.Extensions;
using Q.FilterBuilder.Tests.Shared;

namespace Q.FilterBuilder.MySql.Tests.Extensions;

public class MySqlDapperExtensionsTests
{
    [Fact]
    public void BuildForDapper_SingleParameter()
    {
        // Arrange
        var builder = new TestFilterBuilder("Col=?", ["A"]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForDapper(group);

        // Assert
        Assert.Equal("Col=?", query);
        Assert.Single(parameters);
        Assert.Equal("A", parameters["p0"]);
    }

    [Fact]
    public void BuildForDapper_MultipleParameters()
    {
        // Arrange
        var builder = new TestFilterBuilder("A=? AND B=?", [1, 2]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForDapper(group);

        // Assert
        Assert.Equal("A=? AND B=?", query);
        Assert.Equal(2, parameters.Count);
        Assert.Equal(1, parameters["p0"]);
        Assert.Equal(2, parameters["p1"]);
    }

    [Fact]
    public void BuildForDapper_NoParameters()
    {
        // Arrange
        var builder = new TestFilterBuilder("IsActive=1", Array.Empty<object>());
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForDapper(group);

        // Assert
        Assert.Equal("IsActive=1", query);
        Assert.Empty(parameters);
    }

    [Fact]
    public void BuildForDapper_ExtraParameters_IgnoresExtra()
    {
        // Arrange
        var builder = new TestFilterBuilder("A=?", [1, 2, 3]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForDapper(group);

        // Assert
        Assert.Equal("A=?", query);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(1, parameters["p0"]);
        Assert.Equal(2, parameters["p1"]);
        Assert.Equal(3, parameters["p2"]);
    }

    [Fact]
    public void BuildForDapper_EmptyQueryAndParameters()
    {
        // Arrange
        var builder = new TestFilterBuilder(string.Empty, Array.Empty<object>());
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForDapper(group);

        // Assert
        Assert.Equal(string.Empty, query);
        Assert.Empty(parameters);
    }

    [Fact]
    public void BuildForDapper_ParameterTypes()
    {
        // Arrange
        var builder = new TestFilterBuilder("A=? AND B=?", [1, "str"]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForDapper(group);

        // Assert
        Assert.Equal(1, parameters["p0"]);
        Assert.Equal("str", parameters["p1"]);
    }

    [Fact]
    public void BuildForDapper_ParameterDictionaryIsNew()
    {
        // Arrange
        var builder = new TestFilterBuilder("A=?", [1]);
        var group = new FilterGroup("AND");

        // Act
        var (_, parameters) = builder.BuildForDapper(group);
        parameters["p0"] = 42;

        // Assert
        Assert.Equal(42, parameters["p0"]);
    }
}
