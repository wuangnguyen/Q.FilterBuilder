using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.SqlServer.RuleTransformers;

/// <summary>
/// SQL Server specific implementation of IRuleTransformerService.
/// Extends the base service with SQL Server specific rule transformers.
/// </summary>
public class SqlServerRuleTransformerService : RuleTransformerService
{
    /// <summary>
    /// Initializes a new instance of the SqlServerRuleTransformerService class.
    /// </summary>
    public SqlServerRuleTransformerService()
    {
        RegisterSqlServerTransformers();
    }

    /// <summary>
    /// Registers SQL Server specific rule transformers.
    /// </summary>
    private void RegisterSqlServerTransformers()
    {
        // Register range operators
        RegisterTransformer("between", new BetweenRuleTransformer());
        RegisterTransformer("not_between", new NotBetweenRuleTransformer());

        // Register collection operators
        RegisterTransformer("in", new InRuleTransformer());
        RegisterTransformer("not_in", new NotInRuleTransformer());

        // Register string operators
        RegisterTransformer("contains", new ContainsRuleTransformer());
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
