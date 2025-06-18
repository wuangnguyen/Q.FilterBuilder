using System;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.MySql.Extensions;
using Q.FilterBuilder.Tests.Shared;
using Xunit;

namespace Q.FilterBuilder.MySql.Tests.Extensions;

public class MySqlEfExtensionsTests
{

    [Fact]
    public void BuildForEf_ReplacesSingleParameterCorrectly_AAA()
    {
        // Arrange
        var builder = new TestFilterBuilder("`Name` = ?", ["John"]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForEf(group);

        // Assert
        Assert.Equal("`Name` = {0}", query);
        Assert.Single(parameters);
        Assert.Equal("John", parameters[0]);
    }

    [Fact]
    public void BuildForEf_ReplacesMultipleParametersCorrectly_AAA()
    {
        // Arrange
        var builder = new TestFilterBuilder("`Name` = ? AND `Age` > ?", ["John", 30]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForEf(group);

        // Assert
        Assert.Equal("`Name` = {0} AND `Age` > {1}", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("John", parameters[0]);
        Assert.Equal(30, parameters[1]);
    }

    [Fact]
    public void BuildForEf_LeavesQueryUnchangedIfNoParameters_AAA()
    {
        // Arrange
        var builder = new TestFilterBuilder("`IsActive` = 1", Array.Empty<object>());
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForEf(group);

        // Assert
        Assert.Equal("`IsActive` = 1", query);
        Assert.Empty(parameters);
    }

    [Fact]
    public void BuildForEf_ReplacesAllOccurrences_AAA()
    {
        // Arrange
        var builder = new TestFilterBuilder("`A`=? OR `B`=? OR `C`=?", [1, 2, 3]);
        var group = new FilterGroup("OR");

        // Act
        var (query, parameters) = builder.BuildForEf(group);

        // Assert
        Assert.Equal("`A`={0} OR `B`={1} OR `C`={2}", query);
        Assert.Equal(3, parameters.Length);
        Assert.Equal(1, parameters[0]);
        Assert.Equal(2, parameters[1]);
        Assert.Equal(3, parameters[2]);
    }

    [Fact]
    public void BuildForEf_ParametersArrayIsUnchanged_AAA()
    {
        // Arrange
        var originalParams = new object[] { "x", "y" };
        var builder = new TestFilterBuilder("`X`=? AND `Y`=?", originalParams);
        var group = new FilterGroup("AND");

        // Act
        var (_, parameters) = builder.BuildForEf(group);

        // Assert
        Assert.Same(originalParams, parameters);
    }

    [Fact]
    public void BuildForEf_DoesNotReplaceNonQuestionMarkPatterns_AAA()
    {
        // Arrange
        var builder = new TestFilterBuilder("`Name` = @p0 OR `Other` = ?", ["A", "B"]);
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = builder.BuildForEf(group);

        // Assert
        Assert.Equal("`Name` = @p0 OR `Other` = {0}", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("A", parameters[0]);
        Assert.Equal("B", parameters[1]);
    }
}
