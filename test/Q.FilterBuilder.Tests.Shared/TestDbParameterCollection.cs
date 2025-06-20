using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Q.FilterBuilder.Testing.AdoNet;

[ExcludeFromCodeCoverage]
public class TestDbParameter : DbParameter
{
    public override void ResetDbType() { }
    public override DbType DbType { get; set; }
    public override ParameterDirection Direction { get; set; }
    public override bool IsNullable { get; set; }
    public override string ParameterName { get; set; }
    public override string SourceColumn { get; set; }
    public override object Value { get; set; }
    public override bool SourceColumnNullMapping { get; set; }
    public override int Size { get; set; }
}

public class TestDbParameterCollection : DbParameterCollection
{
    private readonly List<DbParameter> _parameters = new();
    public override int Add(object value) { _parameters.Add((DbParameter)value); return _parameters.Count - 1; }
    public override void AddRange(Array values) { foreach (var v in values) Add(v); }
    public override void Clear() => _parameters.Clear();
    public override bool Contains(object value) => _parameters.Contains((DbParameter)value);
    public override bool Contains(string value) => _parameters.Exists(p => p.ParameterName == value);
    public override void CopyTo(Array array, int index) => _parameters.ToArray().CopyTo(array, index);
    public override int Count => _parameters.Count;
    public override System.Collections.IEnumerator GetEnumerator() => _parameters.GetEnumerator();
    public override int IndexOf(object value) => _parameters.IndexOf((DbParameter)value);
    public override int IndexOf(string parameterName) => _parameters.FindIndex(p => p.ParameterName == parameterName);
    public override void Insert(int index, object value) => _parameters.Insert(index, (DbParameter)value);
    public override bool IsFixedSize => false;
    public override bool IsReadOnly => false;
    public override bool IsSynchronized => false;
    public override void Remove(object value) => _parameters.Remove((DbParameter)value);
    public override void RemoveAt(int index) => _parameters.RemoveAt(index);
    public override void RemoveAt(string parameterName) => _parameters.RemoveAll(p => p.ParameterName == parameterName);
    public override object SyncRoot => this;
    protected override DbParameter GetParameter(int index) => _parameters[index];
    protected override DbParameter GetParameter(string parameterName) => _parameters.Find(p => p.ParameterName == parameterName);
    protected override void SetParameter(int index, DbParameter value) => _parameters[index] = value;
    protected override void SetParameter(string parameterName, DbParameter value) { var idx = IndexOf(parameterName); if (idx >= 0) _parameters[idx] = value; }
    public List<DbParameter> Parameters => _parameters;
}

public class TestDbCommand : DbCommand
{
    public TestDbParameterCollection TestParameters { get; } = new();
    public override string CommandText { get; set; }
    public override int CommandTimeout { get; set; }
    public override CommandType CommandType { get; set; }
    public override bool DesignTimeVisible { get; set; }
    public override UpdateRowSource UpdatedRowSource { get; set; }
    protected override DbConnection DbConnection { get; set; }
    protected override DbParameterCollection DbParameterCollection => TestParameters;
    protected override DbTransaction DbTransaction { get; set; }
    public override void Cancel() { }
    public override int ExecuteNonQuery() => 0;
    public override object ExecuteScalar() => null!;
    public override void Prepare() { }
    protected override DbParameter CreateDbParameter() => new TestDbParameter();
    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => null!;
}
