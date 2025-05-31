using Q.FilterBuilder.Core.Models;
using Xunit;

namespace Q.FilterBuilder.Core.Tests;

public class FluentRuleBuilderTests
{
    [Fact]
    public void Where_WithSingleRule_ShouldAddRuleToTopLevel()
    {
        // Arrange
        var builder = new FluentRuleBuilder();

        // Act
        var result = builder
            .Where("Age", "greater", 18)
            .Build();

        // Assert
        Assert.Single(result.Rules);
        Assert.Equal("Age", result.Rules[0].FieldName);
        Assert.Equal("greater", result.Rules[0].Operator);
        Assert.Equal(18, result.Rules[0].Value);
        Assert.Equal("AND", result.Condition);
    }

    [Fact]
    public void Where_WithMultipleRules_ShouldAddAllRules()
    {
        // Arrange
        var builder = new FluentRuleBuilder();

        // Act
        var result = builder
            .Where("Age", "greater", 18)
            .Where("Status", "equal", "Active")
            .Where("Name", "contains", "John")
            .Build();

        // Assert
        Assert.Equal(3, result.Rules.Count);
        Assert.Equal("Age", result.Rules[0].FieldName);
        Assert.Equal("Status", result.Rules[1].FieldName);
        Assert.Equal("Name", result.Rules[2].FieldName);
    }

    [Fact]
    public void BeginGroup_EndGroup_ShouldCreateNestedGroup()
    {
        // Arrange
        var builder = new FluentRuleBuilder();

        // Act
        var result = builder
            .Where("Age", "greater", 18)
            .BeginGroup("OR")
                .Where("City", "equal", "New York")
                .Where("City", "equal", "Los Angeles")
            .EndGroup()
            .Build();

        // Assert
        Assert.Single(result.Rules);
        Assert.Single(result.Groups);

        var nestedGroup = result.Groups[0];
        Assert.Equal("OR", nestedGroup.Condition);
        Assert.Equal(2, nestedGroup.Rules.Count);
        Assert.Equal("City", nestedGroup.Rules[0].FieldName);
        Assert.Equal("New York", nestedGroup.Rules[0].Value);
        Assert.Equal("Los Angeles", nestedGroup.Rules[1].Value);
    }

    [Fact]
    public void BeginGroup_WithDefaultCondition_ShouldUseAND()
    {
        // Arrange
        var builder = new FluentRuleBuilder();

        // Act
        var result = builder
            .BeginGroup()
                .Where("Field1", "equal", "value1")
            .EndGroup()
            .Build();

        // Assert
        Assert.Single(result.Groups);
        Assert.Equal("AND", result.Groups[0].Condition);
    }

    [Fact]
    public void EndGroup_WithoutBeginGroup_ShouldThrowException()
    {
        // Arrange
        var builder = new FluentRuleBuilder();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => builder.EndGroup());
        Assert.Contains("No group to end. Call BeginGroup first.", exception.Message);
    }

    [Fact]
    public void Build_WithUnclosedGroups_ShouldThrowException()
    {
        // Arrange
        var builder = new FluentRuleBuilder();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            builder.BeginGroup().Where("Field", "equal", "value").Build());

        Assert.Contains("Unclosed groups detected", exception.Message);
        Assert.Contains("Call EndGroup() 1 more time(s)", exception.Message);
    }

    [Fact]
    public void Build_WithMultipleUnclosedGroups_ShouldShowCorrectCount()
    {
        // Arrange
        var builder = new FluentRuleBuilder();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            builder.BeginGroup().BeginGroup().Where("Field", "equal", "value").Build());

        Assert.Contains("Call EndGroup() 2 more time(s)", exception.Message);
    }

    [Fact]
    public void NestedGroups_ShouldSupportMultipleLevels()
    {
        // Arrange
        var builder = new FluentRuleBuilder();

        // Act
        var result = builder
            .Where("TopLevel", "equal", "value")
            .BeginGroup("OR")
                .Where("Level1", "equal", "value1")
                .BeginGroup("AND")
                    .Where("Level2A", "equal", "value2a")
                    .Where("Level2B", "equal", "value2b")
                .EndGroup()
            .EndGroup()
            .Build();

        // Assert
        Assert.Single(result.Rules);
        Assert.Single(result.Groups);

        var level1Group = result.Groups[0];
        Assert.Equal("OR", level1Group.Condition);
        Assert.Single(level1Group.Rules);
        Assert.Single(level1Group.Groups);

        var level2Group = level1Group.Groups[0];
        Assert.Equal("AND", level2Group.Condition);
        Assert.Equal(2, level2Group.Rules.Count);
    }

    [Fact]
    public void Clear_ShouldRemoveAllRulesAndGroups()
    {
        // Arrange
        var builder = new FluentRuleBuilder();
        builder.Where("Field1", "equal", "value1")
               .BeginGroup("OR")
                   .Where("Field2", "equal", "value2")
               .EndGroup();

        // Act
        var clearedBuilder = builder.Clear();
        var result = clearedBuilder.Build();

        // Assert
        Assert.Same(builder, clearedBuilder); // Should return same instance for chaining
        Assert.Empty(result.Rules);
        Assert.Empty(result.Groups);
    }

    [Fact]
    public void Build_WithCustomCondition_ShouldUseSpecifiedCondition()
    {
        // Arrange
        var builder = new FluentRuleBuilder();

        // Act
        var result = builder
            .Where("Field1", "equal", "value1")
            .Where("Field2", "equal", "value2")
            .Build("OR");

        // Assert
        Assert.Equal("OR", result.Condition);
        Assert.Equal(2, result.Rules.Count);
    }

    [Fact]
    public void ConcatGroups_WithValidGroups_ShouldCombineGroups()
    {
        // Arrange
        var group1 = new FluentRuleBuilder()
            .Where("Age", "greater", 18)
            .Where("Status", "equal", "Active")
            .Build();

        var group2 = new FluentRuleBuilder()
            .Where("City", "equal", "New York")
            .Where("State", "equal", "NY")
            .Build();

        // Act
        var result = FluentRuleBuilder.ConcatGroups(group1, group2, "AND");

        // Assert
        Assert.Equal("AND", result.Condition);
        Assert.Equal(2, result.Groups.Count);
        Assert.Same(group1, result.Groups[0]);
        Assert.Same(group2, result.Groups[1]);
        Assert.Empty(result.Rules);
    }

    [Fact]
    public void ConcatGroups_WithDefaultCondition_ShouldUseAND()
    {
        // Arrange
        var group1 = new FilterGroup("AND");
        var group2 = new FilterGroup("OR");

        // Act
        var result = FluentRuleBuilder.ConcatGroups(group1, group2);

        // Assert
        Assert.Equal("AND", result.Condition);
        Assert.Equal(2, result.Groups.Count);
    }

    [Fact]
    public void ConcatGroups_WithNullFirstGroup_ShouldThrowArgumentNullException()
    {
        // Arrange
        var group2 = new FilterGroup("OR");

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            FluentRuleBuilder.ConcatGroups(null!, group2));

        Assert.Equal("group1", exception.ParamName);
    }

    [Fact]
    public void ConcatGroups_WithNullSecondGroup_ShouldThrowArgumentNullException()
    {
        // Arrange
        var group1 = new FilterGroup("AND");

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            FluentRuleBuilder.ConcatGroups(group1, null!));

        Assert.Equal("group2", exception.ParamName);
    }

    [Fact]
    public void AddGroup_WithValidGroup_ShouldAddToTopLevel()
    {
        // Arrange
        var builder = new FluentRuleBuilder();
        var existingGroup = new FilterGroup("OR");
        existingGroup.Rules.Add(new FilterRule("Field", "equal", "value"));

        // Act
        var result = builder
            .Where("TopField", "equal", "topValue")
            .AddGroup(existingGroup)
            .Build();

        // Assert
        Assert.Single(result.Rules);
        Assert.Single(result.Groups);
        Assert.Same(existingGroup, result.Groups[0]);
    }

    [Fact]
    public void AddGroup_WithinNestedGroup_ShouldAddToCurrentGroup()
    {
        // Arrange
        var builder = new FluentRuleBuilder();
        var existingGroup = new FilterGroup("OR");
        existingGroup.Rules.Add(new FilterRule("Field", "equal", "value"));

        // Act
        var result = builder
            .BeginGroup("AND")
                .Where("NestedField", "equal", "nestedValue")
                .AddGroup(existingGroup)
            .EndGroup()
            .Build();

        // Assert
        Assert.Single(result.Groups);
        var nestedGroup = result.Groups[0];
        Assert.Single(nestedGroup.Rules);
        Assert.Single(nestedGroup.Groups);
        Assert.Same(existingGroup, nestedGroup.Groups[0]);
    }

    [Fact]
    public void AddGroup_WithNullGroup_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new FluentRuleBuilder();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => builder.AddGroup(null!));
        Assert.Equal("group", exception.ParamName);
    }

    [Fact]
    public void ComplexScenario_ShouldBuildCorrectStructure()
    {
        // Arrange
        var builder = new FluentRuleBuilder();
        var existingGroup = new FilterGroup("OR");
        existingGroup.Rules.Add(new FilterRule("ExistingField", "equal", "existingValue"));

        // Act
        var result = builder
            .Where("TopLevel", "equal", "topValue")
            .BeginGroup("OR")
                .Where("Level1A", "equal", "value1a")
                .BeginGroup("AND")
                    .Where("Level2A", "equal", "value2a")
                    .Where("Level2B", "greater", 10)
                .EndGroup()
                .AddGroup(existingGroup)
            .EndGroup()
            .Where("AnotherTop", "contains", "search")
            .Build();

        // Assert
        Assert.Equal("AND", result.Condition);
        Assert.Equal(2, result.Rules.Count);
        Assert.Single(result.Groups);

        var mainNestedGroup = result.Groups[0];
        Assert.Equal("OR", mainNestedGroup.Condition);
        Assert.Single(mainNestedGroup.Rules);
        Assert.Equal(2, mainNestedGroup.Groups.Count);

        var deepNestedGroup = mainNestedGroup.Groups[0];
        Assert.Equal("AND", deepNestedGroup.Condition);
        Assert.Equal(2, deepNestedGroup.Rules.Count);

        var addedGroup = mainNestedGroup.Groups[1];
        Assert.Same(existingGroup, addedGroup);
    }

    [Fact]
    public void Where_WithNullValue_ShouldAcceptNullValue()
    {
        // Arrange
        var builder = new FluentRuleBuilder();

        // Act
        var result = builder
            .Where("Field", "is_null", null)
            .Build();

        // Assert
        Assert.Single(result.Rules);
        Assert.Null(result.Rules[0].Value);
    }

    [Fact]
    public void Where_WithExplicitType_ShouldSetType()
    {
        // Arrange
        var builder = new FluentRuleBuilder();

        // Act
        var result = builder
            .Where("Age", "greater", "25", "int")
            .Build();

        // Assert
        Assert.Single(result.Rules);
        Assert.Equal("int", result.Rules[0].Type);
        Assert.Equal("25", result.Rules[0].Value);
    }

    [Fact]
    public void Where_WithoutExplicitType_ShouldSetEmptyType()
    {
        // Arrange
        var builder = new FluentRuleBuilder();

        // Act
        var result = builder
            .Where("Name", "equal", "John")
            .Build();

        // Assert
        Assert.Single(result.Rules);
        Assert.Equal(string.Empty, result.Rules[0].Type);
    }

    [Fact]
    public void Where_WithComplexValue_ShouldStoreComplexValue()
    {
        // Arrange
        var builder = new FluentRuleBuilder();
        var complexValue = new { Name = "John", Age = 25 };

        // Act
        var result = builder
            .Where("User", "equal", complexValue)
            .Build();

        // Assert
        Assert.Single(result.Rules);
        Assert.Same(complexValue, result.Rules[0].Value);
    }

    [Fact]
    public void Where_WithArrayValue_ShouldStoreArrayValue()
    {
        // Arrange
        var builder = new FluentRuleBuilder();
        var arrayValue = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = builder
            .Where("Numbers", "in", arrayValue)
            .Build();

        // Assert
        Assert.Single(result.Rules);
        Assert.Same(arrayValue, result.Rules[0].Value);
    }

    [Fact]
    public void Build_WithEmptyBuilder_ShouldReturnEmptyGroup()
    {
        // Arrange
        var builder = new FluentRuleBuilder();

        // Act
        var result = builder.Build();

        // Assert
        Assert.Equal("AND", result.Condition);
        Assert.Empty(result.Rules);
        Assert.Empty(result.Groups);
    }

    [Fact]
    public void Clear_WithUnclosedGroups_ShouldClearGroupStack()
    {
        // Arrange
        var builder = new FluentRuleBuilder();
        builder.BeginGroup("OR").Where("Field", "equal", "value");

        // Act
        var clearedBuilder = builder.Clear();
        var result = clearedBuilder.Build(); // Should not throw

        // Assert
        Assert.Empty(result.Rules);
        Assert.Empty(result.Groups);
    }

    [Fact]
    public void FluentChaining_ShouldReturnSameInstance()
    {
        // Arrange
        var builder = new FluentRuleBuilder();

        // Act & Assert - All methods should return the same instance for chaining
        Assert.Same(builder, builder.Where("Field1", "equal", "value1"));
        Assert.Same(builder, builder.BeginGroup("OR"));
        Assert.Same(builder, builder.Where("Field2", "equal", "value2"));
        Assert.Same(builder, builder.EndGroup());
        Assert.Same(builder, builder.AddGroup(new FilterGroup("AND")));
        Assert.Same(builder, builder.Clear());
    }
}
