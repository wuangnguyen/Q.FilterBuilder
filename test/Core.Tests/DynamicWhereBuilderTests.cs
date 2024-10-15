using DynamicWhere.Core.Models;
using DynamicWhere.Core;
using Shouldly;
using DynamicWhere.Core.Providers;

namespace Core.Tests;

public class DynamicWhereBuilderTests
{
    private readonly IOperatorProvider mockOperatorProvider;

    public DynamicWhereBuilderTests()
    {
        mockOperatorProvider = new MockOperatorProvider();
    }

    [Fact]
    public void Build_InputisNull_ShouldThrowException()
    {
        // Act & Assert
        Should.Throw<Exception>(() =>
        {
            new DynamicWhereBuilder().Build(null!);
        });
    }

    [Fact]
    public void Build_SingleRule_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var group = new DynamicGroup
        {
            Condition = "AND",
            Rules =
            [
                new DynamicRule
                {
                    FieldName = "Name",
                    Operator = "equal",
                    Value = "John"
                }
            ]
        };

        var builder = new DynamicWhereBuilder(mockOperatorProvider);

        // Act
        var (query, parameters) = builder.Build(group);

        // Assert
        query.ShouldBe("Name == @0");
        parameters.ShouldBe(new object[] { "John" });
    }

    [Fact]
    public void Build_MultipleRules_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var group = new DynamicGroup
        {
            Condition = "AND",
            Rules =
            [
                new DynamicRule
                {
                    FieldName = "Name",
                    Operator = "equal",
                    Value = "John"
                },
                new DynamicRule
                {
                    FieldName = "Age",
                    Operator = "greater",
                    Value = 30
                }
            ]
        };

        var builder = new DynamicWhereBuilder(mockOperatorProvider);

        // Act
        var (query, parameters) = builder.Build(group);

        // Assert
        query.ShouldBe("Name == @0 AND Age > @1");
        parameters.ShouldBe(new object[] { "John", 30 });
    }

    [Fact]
    public void Build_NestedGroups_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var group = new DynamicGroup
        {
            Condition = "OR",
            Groups =
        [
            new DynamicGroup
            {
                Condition = "AND",
                Rules =
                [
                    new DynamicRule
                    {
                        FieldName = "Name",
                        Operator = "equal",
                        Value = "John"
                    }
                ]
            },
            new DynamicGroup
            {
                Condition = "OR",
                Rules =
                [
                    new DynamicRule
                    {
                        FieldName = "Age",
                        Operator = "greater",
                        Type = "integer",
                        Value = 30
                    }
                ]
            }
        ]
        };

        var builder = new DynamicWhereBuilder(mockOperatorProvider);

        // Act
        var (query, parameters) = builder.Build(group);

        // Assert
        query.ShouldBe("(Name == @0) OR (Age > @1)");
        parameters.ShouldBe(new object[] { "John", 30 });
    }

    [Fact]
    public void Build_EmptyGroup_ShouldGenerateEmptyQuery()
    {
        // Arrange
        var group = new DynamicGroup
        {
            Condition = "AND",
            Rules = []
        };

        var builder = new DynamicWhereBuilder(mockOperatorProvider);

        // Act
        var (query, parameters) = builder.Build(group);

        // Assert
        query.ShouldBe("");
        parameters.ShouldBe(Array.Empty<object>());
    }

    [Fact]
    public void Build_GroupWithSubGroups_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var group = new DynamicGroup
        {
            Condition = "AND",
            Rules =
            [
                new DynamicRule
                {
                    FieldName = "Status",
                    Operator = "equal",
                    Value = "Active"
                }
            ],
            Groups =
            [
                new DynamicGroup
                {
                    Condition = "OR",
                    Rules =
                    [
                        new DynamicRule
                        {
                            FieldName = "Priority",
                            Operator = "greater",
                            Value = 5
                        }
                    ]
                }
            ]
        };

        var builder = new DynamicWhereBuilder(mockOperatorProvider);

        // Act
        var (query, parameters) = builder.Build(group);

        // Assert
        query.ShouldBe("Status == @0 AND (Priority > @1)");
        parameters.ShouldBe(new object[] { "Active", 5 });
    }

    [Fact]
    public void Build_GroupWithMultipleSubGroups_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var group = new DynamicGroup
        {
            Condition = "OR",
            Groups =
            [
                new DynamicGroup
                {
                    Condition = "AND",
                    Rules =
                    [
                        new DynamicRule
                        {
                            FieldName = "Name",
                            Operator = "equal",
                            Value = "John"
                        }
                    ]
                },
                new DynamicGroup
                {
                    Condition = "AND",
                    Rules =
                    [
                        new DynamicRule
                        {
                            FieldName = "Age",
                            Operator = "greater",
                            Value = 30
                        }
                    ]
                }
            ]
        };

        var builder = new DynamicWhereBuilder(mockOperatorProvider);

        // Act
        var (query, parameters) = builder.Build(group);

        // Assert
        query.ShouldBe("(Name == @0) OR (Age > @1)");
        parameters.ShouldBe(new object[] { "John", 30 });
    }

    // TODO: add more case where combine simple operator and between operator
}