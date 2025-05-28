using Q.FilterBuilder.Core.Models;
using Xunit;

namespace Q.FilterBuilder.Core.Tests.Models;

public class FilterGroupTests
{
    [Fact]
    public void Constructor_WithValidCondition_ShouldCreateInstance()
    {
        // Arrange
        var condition = "AND";

        // Act
        var group = new FilterGroup(condition);

        // Assert
        Assert.Equal(condition, group.Condition);
        Assert.NotNull(group.Rules);
        Assert.Empty(group.Rules);
        Assert.NotNull(group.Groups);
        Assert.Empty(group.Groups);
    }

    [Fact]
    public void Constructor_WithDefaultCondition_ShouldUseAND()
    {
        // Act
        var group = new FilterGroup("AND");

        // Assert
        Assert.Equal("AND", group.Condition);
        Assert.NotNull(group.Rules);
        Assert.Empty(group.Rules);
        Assert.NotNull(group.Groups);
        Assert.Empty(group.Groups);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidCondition_ShouldThrowArgumentNullException(string? condition)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new FilterGroup(condition!));

        Assert.Equal("condition", exception.ParamName);
        Assert.Contains("Condition cannot be null or whitespace", exception.Message);
    }

    [Fact]
    public void Rules_ShouldAllowAddingRules()
    {
        // Arrange
        var group = new FilterGroup("AND");
        var rule1 = new FilterRule("Field1", "equal", "value1");
        var rule2 = new FilterRule("Field2", "greater", 10);

        // Act
        group.Rules.Add(rule1);
        group.Rules.Add(rule2);

        // Assert
        Assert.Equal(2, group.Rules.Count);
        Assert.Contains(rule1, group.Rules);
        Assert.Contains(rule2, group.Rules);
    }

    [Fact]
    public void Groups_ShouldAllowAddingNestedGroups()
    {
        // Arrange
        var parentGroup = new FilterGroup("AND");
        var childGroup1 = new FilterGroup("OR");
        var childGroup2 = new FilterGroup("AND");

        // Act
        parentGroup.Groups.Add(childGroup1);
        parentGroup.Groups.Add(childGroup2);

        // Assert
        Assert.Equal(2, parentGroup.Groups.Count);
        Assert.Contains(childGroup1, parentGroup.Groups);
        Assert.Contains(childGroup2, parentGroup.Groups);
    }

    [Fact]
    public void ComplexStructure_ShouldSupportMixedRulesAndGroups()
    {
        // Arrange
        var mainGroup = new FilterGroup("AND");
        var rule1 = new FilterRule("Age", "greater", 18);
        var rule2 = new FilterRule("Status", "equal", "Active");

        var nestedGroup = new FilterGroup("OR");
        var nestedRule1 = new FilterRule("City", "equal", "New York");
        var nestedRule2 = new FilterRule("City", "equal", "Los Angeles");

        nestedGroup.Rules.Add(nestedRule1);
        nestedGroup.Rules.Add(nestedRule2);

        // Act
        mainGroup.Rules.Add(rule1);
        mainGroup.Rules.Add(rule2);
        mainGroup.Groups.Add(nestedGroup);

        // Assert
        Assert.Equal(2, mainGroup.Rules.Count);
        Assert.Single(mainGroup.Groups);
        Assert.Equal(2, mainGroup.Groups[0].Rules.Count);
        Assert.Equal("OR", mainGroup.Groups[0].Condition);
    }

    [Theory]
    [InlineData("AND")]
    [InlineData("OR")]
    [InlineData("and")]
    [InlineData("or")]
    [InlineData("CUSTOM")]
    public void Constructor_WithVariousConditions_ShouldAcceptAnyNonEmptyString(string condition)
    {
        // Act
        var group = new FilterGroup(condition);

        // Assert
        Assert.Equal(condition, group.Condition);
    }

    [Fact]
    public void Rules_ShouldBeModifiableCollection()
    {
        // Arrange
        var group = new FilterGroup("AND");
        var rule = new FilterRule("Field", "equal", "value");

        // Act
        group.Rules.Add(rule);
        group.Rules.Remove(rule);

        // Assert
        Assert.Empty(group.Rules);
    }

    [Fact]
    public void Groups_ShouldBeModifiableCollection()
    {
        // Arrange
        var parentGroup = new FilterGroup("AND");
        var childGroup = new FilterGroup("OR");

        // Act
        parentGroup.Groups.Add(childGroup);
        parentGroup.Groups.Remove(childGroup);

        // Assert
        Assert.Empty(parentGroup.Groups);
    }

    [Fact]
    public void EmptyGroup_ShouldBeValid()
    {
        // Act
        var group = new FilterGroup("AND");

        // Assert
        Assert.Empty(group.Rules);
        Assert.Empty(group.Groups);
        Assert.Equal("AND", group.Condition);
    }

    [Fact]
    public void DeepNesting_ShouldBeSupported()
    {
        // Arrange
        var level1 = new FilterGroup("AND");
        var level2 = new FilterGroup("OR");
        var level3 = new FilterGroup("AND");
        var deepRule = new FilterRule("DeepField", "equal", "DeepValue");

        // Act
        level3.Rules.Add(deepRule);
        level2.Groups.Add(level3);
        level1.Groups.Add(level2);

        // Assert
        Assert.Single(level1.Groups);
        Assert.Single(level1.Groups[0].Groups);
        Assert.Single(level1.Groups[0].Groups[0].Rules);
        Assert.Equal("DeepField", level1.Groups[0].Groups[0].Rules[0].FieldName);
    }
}
