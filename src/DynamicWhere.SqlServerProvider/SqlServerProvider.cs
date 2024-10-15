using DynamicWhere.Core.Operators;
using DynamicWhere.Core.Providers;
using DynamicWhere.SqlServerProvider.Operators;

namespace DynamicWhere.SqlServerProvider;

public class SqlServerProvider: BaseOperatorProvider
{
    public SqlServerProvider()
    {
        AddOperator("equal", new SimpleOperator("="));
        AddOperator("between", new BetweenOperator());
        AddOperator("not_between", new NotBetweenOperator());
        AddOperator("in", new InOperator());
        AddOperator("not_in", new NotInOperator());
        AddOperator("begins_with", new BeginsWithOperator());
        AddOperator("not_begins_with", new NotBeginsWithOperator());
        AddOperator("contains", new ContainsOperator());
        AddOperator("contains_any", new ContainsOperator());
        AddOperator("not_contains", new NotContainsOperator());
        AddOperator("ends_with", new EndsWithOperator());
        AddOperator("not_ends_with", new NotEndsWithOperator());
        AddOperator("is_empty", new IsEmptyOperator());
        AddOperator("is_not_empty", new IsNotEmptyOperator());
        AddOperator("is_null", new IsNullOperator());
        AddOperator("is_not_null", new IsNotNullOperator());
        AddOperator("date_diff", new DateDiffOperator());
    }
}
