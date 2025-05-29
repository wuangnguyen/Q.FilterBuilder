using System;
using Q.FilterBuilder.SqlServer.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests.RuleTransformers;

public class SqlServerRuleTransformerServiceTests
{
    private readonly SqlServerRuleTransformerService _service = new();

    [Fact]
    public void GetRuleTransformer_WithIsNull_ShouldReturnIsNullTransformer()
    {
        // Act
        var transformer = _service.GetRuleTransformer("is_null");

        // Assert
        Assert.IsType<IsNullRuleTransformer>(transformer);
    }

    [Fact]
    public void GetRuleTransformer_WithIsNotNull_ShouldReturnIsNotNullTransformer()
    {
        // Act
        var transformer = _service.GetRuleTransformer("is_not_null");

        // Assert
        Assert.IsType<IsNotNullRuleTransformer>(transformer);
    }

    [Fact]
    public void GetRuleTransformer_WithIsEmpty_ShouldReturnIsEmptyTransformer()
    {
        // Act
        var transformer = _service.GetRuleTransformer("is_empty");

        // Assert
        Assert.IsType<IsEmptyRuleTransformer>(transformer);
    }

    [Fact]
    public void GetRuleTransformer_WithIsNotEmpty_ShouldReturnIsNotEmptyTransformer()
    {
        // Act
        var transformer = _service.GetRuleTransformer("is_not_empty");

        // Assert
        Assert.IsType<IsNotEmptyRuleTransformer>(transformer);
    }

    [Fact]
    public void GetRuleTransformer_WithContains_ShouldReturnContainsTransformer()
    {
        // Act
        var transformer = _service.GetRuleTransformer("contains");

        // Assert
        Assert.IsType<ContainsRuleTransformer>(transformer);
    }

    [Fact]
    public void GetRuleTransformer_WithNotContains_ShouldReturnNotContainsTransformer()
    {
        // Act
        var transformer = _service.GetRuleTransformer("not_contains");

        // Assert
        Assert.IsType<NotContainsRuleTransformer>(transformer);
    }

    [Fact]
    public void GetRuleTransformer_WithBeginsWith_ShouldReturnBeginsWithTransformer()
    {
        // Act
        var transformer = _service.GetRuleTransformer("begins_with");

        // Assert
        Assert.IsType<BeginsWithRuleTransformer>(transformer);
    }

    [Fact]
    public void GetRuleTransformer_WithNotBeginsWith_ShouldReturnNotBeginsWithTransformer()
    {
        // Act
        var transformer = _service.GetRuleTransformer("not_begins_with");

        // Assert
        Assert.IsType<NotBeginsWithRuleTransformer>(transformer);
    }

    [Fact]
    public void GetRuleTransformer_WithEndsWith_ShouldReturnEndsWithTransformer()
    {
        // Act
        var transformer = _service.GetRuleTransformer("ends_with");

        // Assert
        Assert.IsType<EndsWithRuleTransformer>(transformer);
    }

    [Fact]
    public void GetRuleTransformer_WithNotEndsWith_ShouldReturnNotEndsWithTransformer()
    {
        // Act
        var transformer = _service.GetRuleTransformer("not_ends_with");

        // Assert
        Assert.IsType<NotEndsWithRuleTransformer>(transformer);
    }

    [Fact]
    public void GetRuleTransformer_WithBetween_ShouldReturnBetweenTransformer()
    {
        // Act
        var transformer = _service.GetRuleTransformer("between");

        // Assert
        Assert.IsType<BetweenRuleTransformer>(transformer);
    }

    [Fact]
    public void GetRuleTransformer_WithNotBetween_ShouldReturnNotBetweenTransformer()
    {
        // Act
        var transformer = _service.GetRuleTransformer("not_between");

        // Assert
        Assert.IsType<NotBetweenRuleTransformer>(transformer);
    }

    [Fact]
    public void GetRuleTransformer_WithIn_ShouldReturnInTransformer()
    {
        // Act
        var transformer = _service.GetRuleTransformer("in");

        // Assert
        Assert.IsType<InRuleTransformer>(transformer);
    }

    [Fact]
    public void GetRuleTransformer_WithNotIn_ShouldReturnNotInTransformer()
    {
        // Act
        var transformer = _service.GetRuleTransformer("not_in");

        // Assert
        Assert.IsType<NotInRuleTransformer>(transformer);
    }

    [Fact]
    public void GetRuleTransformer_WithDateDiff_ShouldReturnDateDiffTransformer()
    {
        // Act
        var transformer = _service.GetRuleTransformer("date_diff");

        // Assert
        Assert.IsType<DateDiffRuleTransformer>(transformer);
    }

    [Fact]
    public void GetRuleTransformer_WithCaseInsensitiveOperator_ShouldReturnCorrectTransformer()
    {
        // Act
        var transformer1 = _service.GetRuleTransformer("IS_NULL");
        var transformer2 = _service.GetRuleTransformer("Contains");
        var transformer3 = _service.GetRuleTransformer("BETWEEN");
        var transformer4 = _service.GetRuleTransformer("Date_Diff");

        // Assert
        Assert.IsType<IsNullRuleTransformer>(transformer1);
        Assert.IsType<ContainsRuleTransformer>(transformer2);
        Assert.IsType<BetweenRuleTransformer>(transformer3);
        Assert.IsType<DateDiffRuleTransformer>(transformer4);
    }

    [Fact]
    public void GetRuleTransformer_WithMixedCaseOperators_ShouldReturnCorrectTransformers()
    {
        // Act
        var transformer1 = _service.GetRuleTransformer("Is_Not_Null");
        var transformer2 = _service.GetRuleTransformer("BEGINS_with");
        var transformer3 = _service.GetRuleTransformer("ends_WITH");
        var transformer4 = _service.GetRuleTransformer("Not_Contains");

        // Assert
        Assert.IsType<IsNotNullRuleTransformer>(transformer1);
        Assert.IsType<BeginsWithRuleTransformer>(transformer2);
        Assert.IsType<EndsWithRuleTransformer>(transformer3);
        Assert.IsType<NotContainsRuleTransformer>(transformer4);
    }

    [Fact]
    public void GetRuleTransformer_WithAllSupportedOperators_ShouldReturnCorrectTransformers()
    {
        // Arrange
        var operatorMappings = new[]
        {
            ("is_null", typeof(IsNullRuleTransformer)),
            ("is_not_null", typeof(IsNotNullRuleTransformer)),
            ("is_empty", typeof(IsEmptyRuleTransformer)),
            ("is_not_empty", typeof(IsNotEmptyRuleTransformer)),
            ("contains", typeof(ContainsRuleTransformer)),
            ("not_contains", typeof(NotContainsRuleTransformer)),
            ("begins_with", typeof(BeginsWithRuleTransformer)),
            ("not_begins_with", typeof(NotBeginsWithRuleTransformer)),
            ("ends_with", typeof(EndsWithRuleTransformer)),
            ("not_ends_with", typeof(NotEndsWithRuleTransformer)),
            ("between", typeof(BetweenRuleTransformer)),
            ("not_between", typeof(NotBetweenRuleTransformer)),
            ("in", typeof(InRuleTransformer)),
            ("not_in", typeof(NotInRuleTransformer)),
            ("date_diff", typeof(DateDiffRuleTransformer))
        };

        foreach (var (operatorName, expectedType) in operatorMappings)
        {
            // Act
            var transformer = _service.GetRuleTransformer(operatorName);

            // Assert
            Assert.IsType(expectedType, transformer);
        }
    }

    [Fact]
    public void GetRuleTransformer_WithUnknownOperator_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        var exception = Assert.Throws<NotImplementedException>(() => _service.GetRuleTransformer("unknown_operator"));
        Assert.Contains("Rule transformer for operator 'unknown_operator' is not implemented", exception.Message);
    }

    [Fact]
    public void GetRuleTransformer_WithNullOperator_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _service.GetRuleTransformer(null!));
    }

    [Fact]
    public void GetRuleTransformer_WithEmptyOperator_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _service.GetRuleTransformer(""));
    }

    [Fact]
    public void GetRuleTransformer_WithWhitespaceOperator_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _service.GetRuleTransformer("   "));
    }

    [Fact]
    public void GetRuleTransformer_WithTabsAndSpaces_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _service.GetRuleTransformer("\t\n\r "));
    }

    [Fact]
    public void GetRuleTransformer_WithSimilarButInvalidOperators_ShouldThrowNotImplementedException()
    {
        // Arrange
        var invalidOperators = new[]
        {
            "is_null_or_empty",
            "contains_any",
            "begins_with_any",
            "ends_with_all",
            "between_inclusive",
            "in_range",
            "date_diff_hours",
            "not_like",
            "equals",
            "not_equals"
        };

        foreach (var invalidOperator in invalidOperators)
        {
            // Act & Assert
            var exception = Assert.Throws<NotImplementedException>(() => _service.GetRuleTransformer(invalidOperator));
            Assert.Contains($"Rule transformer for operator '{invalidOperator}' is not implemented", exception.Message);
        }
    }

    [Fact]
    public void GetRuleTransformer_WithOperatorsWithExtraSpaces_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        var exception1 = Assert.Throws<NotImplementedException>(() => _service.GetRuleTransformer(" is_null"));
        var exception2 = Assert.Throws<NotImplementedException>(() => _service.GetRuleTransformer("is_null "));
        var exception3 = Assert.Throws<NotImplementedException>(() => _service.GetRuleTransformer(" is_null "));

        Assert.Contains("Rule transformer for operator ' is_null' is not implemented", exception1.Message);
        Assert.Contains("Rule transformer for operator 'is_null ' is not implemented", exception2.Message);
        Assert.Contains("Rule transformer for operator ' is_null ' is not implemented", exception3.Message);
    }

    [Fact]
    public void GetRuleTransformer_MultipleCallsWithSameOperator_ShouldReturnSameInstance()
    {
        // Act
        var transformer1 = _service.GetRuleTransformer("is_null");
        var transformer2 = _service.GetRuleTransformer("is_null");

        // Assert
        Assert.IsType<IsNullRuleTransformer>(transformer1);
        Assert.IsType<IsNullRuleTransformer>(transformer2);
        Assert.Same(transformer1, transformer2); // Should be same instance for performance
    }

    [Fact]
    public void GetRuleTransformer_WithSpecialCharactersInOperator_ShouldThrowNotImplementedException()
    {
        // Arrange
        var invalidOperators = new[]
        {
            "is@null",
            "contains#text",
            "begins$with",
            "ends%with",
            "between&values",
            "in(values)",
            "date-diff",
            "not.contains"
        };

        foreach (var invalidOperator in invalidOperators)
        {
            // Act & Assert
            var exception = Assert.Throws<NotImplementedException>(() => _service.GetRuleTransformer(invalidOperator));
            Assert.Contains($"Rule transformer for operator '{invalidOperator}' is not implemented", exception.Message);
        }
    }
}
