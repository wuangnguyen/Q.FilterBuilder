using System;
using System.Diagnostics.CodeAnalysis;
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Core.Providers;

namespace Q.FilterBuilder.Tests.Shared
{
    [ExcludeFromCodeCoverage]
    public class TestFilterBuilder : IFilterBuilder
    {
        private readonly string _query;
        private readonly object[] _parameters;
        public TestFilterBuilder(string query, object[] parameters)
        {
            _query = query;
            _parameters = parameters;
        }
        public IQueryFormatProvider QueryFormatProvider => throw new NotImplementedException();
        public (string parsedQuery, object[] parameters) Build(FilterGroup group) => (_query, _parameters);
    }
}
