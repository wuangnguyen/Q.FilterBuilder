using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.Linq.RuleTransformers;

/// <summary>
/// LINQ specific implementation of IRuleTransformerService.
/// Extends the base service with LINQ specific rule transformers.
/// </summary>
public class LinqRuleTransformerService : RuleTransformerService
{
    /// <summary>
    /// Initializes a new instance of the LinqRuleTransformerService class.
    /// </summary>
    public LinqRuleTransformerService()
    {
        RegisterLinqTransformers();
    }

    /// <summary>
    /// Registers LINQ specific rule transformers.
    /// </summary>
    private void RegisterLinqTransformers()
    {
        // Register range operators
        RegisterTransformer("between", new BetweenRuleTransformer());
        RegisterTransformer("not_between", new NotBetweenRuleTransformer());

        // Register collection operators
        RegisterTransformer("in", new InRuleTransformer());
        RegisterTransformer("not_in", new NotInRuleTransformer());

        // Register string operators
        RegisterTransformer("contains", new ContainsRuleTransformer());
        RegisterTransformer("contains_any", new ContainsRuleTransformer()); // Alias for contains
        RegisterTransformer("not_contains", new NotContainsRuleTransformer());
        RegisterTransformer("begins_with", new BeginsWithRuleTransformer());
        RegisterTransformer("not_begins_with", new NotBeginsWithRuleTransformer());
        RegisterTransformer("ends_with", new EndsWithRuleTransformer());
        RegisterTransformer("not_ends_with", new NotEndsWithRuleTransformer());

        // Register null check operators
        RegisterTransformer("is_null", new IsNullRuleTransformer());
        RegisterTransformer("is_not_null", new IsNotNullRuleTransformer());
        RegisterTransformer("is_empty", new IsEmptyRuleTransformer());
        RegisterTransformer("is_not_empty", new IsNotEmptyRuleTransformer());

        // Register date operators
        RegisterTransformer("date_diff", new DateDiffRuleTransformer());
    }
}
