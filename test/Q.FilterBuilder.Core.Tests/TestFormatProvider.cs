using Q.FilterBuilder.Core.Providers;

namespace Q.FilterBuilder.Core.Tests
{
    internal class TestFormatProvider : IQueryFormatProvider
    {
        public string FormatFieldName(string fieldName) => fieldName;
        public string FormatParameterName(int index) => $"@p{index}";
        public string ParameterPrefix => "@";
        public string AndOperator => " AND ";
        public string OrOperator => " OR ";
    }
}
