using System;
using Q.FilterBuilder.Linq.RuleTransformers;
using Q.FilterBuilder.Core.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.Linq.Tests.RuleTransformers;

public class LinqRuleTransformerServiceTests
{
    [Theory]
    [InlineData("between", typeof(BetweenRuleTransformer))]
    [InlineData("not_between", typeof(NotBetweenRuleTransformer))]
    [InlineData("in", typeof(InRuleTransformer))]
    [InlineData("not_in", typeof(NotInRuleTransformer))]
    [InlineData("contains", typeof(ContainsRuleTransformer))]
    [InlineData("contains_any", typeof(ContainsRuleTransformer))]
    [InlineData("not_contains", typeof(NotContainsRuleTransformer))]
    [InlineData("begins_with", typeof(BeginsWithRuleTransformer))]
    [InlineData("not_begins_with", typeof(NotBeginsWithRuleTransformer))]
    [InlineData("ends_with", typeof(EndsWithRuleTransformer))]
    [InlineData("not_ends_with", typeof(NotEndsWithRuleTransformer))]
    [InlineData("is_null", typeof(IsNullRuleTransformer))]
    [InlineData("is_not_null", typeof(IsNotNullRuleTransformer))]
    [InlineData("is_empty", typeof(IsEmptyRuleTransformer))]
    [InlineData("is_not_empty", typeof(IsNotEmptyRuleTransformer))]
    [InlineData("date_diff", typeof(DateDiffRuleTransformer))]
    public void GetRuleTransformer_ReturnsCorrectType(string op, Type expectedType)
    {
        // Arrange
        var service = new LinqRuleTransformerService();

        // Act
        var transformer = service.GetRuleTransformer(op);

        // Assert
        Assert.IsType(expectedType, transformer);
    }

    [Fact]
    public void GetRuleTransformer_IsCaseInsensitive()
    {
        // Arrange
        var service = new LinqRuleTransformerService();

        // Act & Assert
        Assert.IsType<ContainsRuleTransformer>(service.GetRuleTransformer("CONTAINS"));
        Assert.IsType<NotInRuleTransformer>(service.GetRuleTransformer("Not_In"));
    }

    [Fact]
    public void GetRuleTransformer_UnregisteredOperator_Throws()
    {
        // Arrange
        var service = new LinqRuleTransformerService();

        // Act & Assert
        Assert.Throws<NotImplementedException>(() => service.GetRuleTransformer("unknown_operator"));
    }

    [Fact]
    public void RegisterTransformer_OverridesPrevious()
    {
        // Arrange
        var service = new LinqRuleTransformerService();
        var custom = new CustomRuleTransformer();
        service.RegisterTransformer("contains", custom);

        // Act
        var transformer = service.GetRuleTransformer("contains");

        // Assert
        Assert.Same(custom, transformer);
    }

    private class CustomRuleTransformer : IRuleTransformer
    {
        public (string query, object[]? parameters) Transform(Core.Models.FilterRule rule, string fieldName, int parameterIndex, Core.Providers.IQueryFormatProvider formatProvider)
            => ("custom", null);
    }
} 
