using System;
using System.Data.Common;
using Q.FilterBuilder.PostgreSql.Extensions;
using Q.FilterBuilder.Testing.AdoNet;
using Xunit;

namespace Q.FilterBuilder.PostgreSql.Tests.Extensions;

public class PostgreSqlAdoNetExtensionsTests
{
    [Fact]
    public void AddParameters_AddsParametersWithValues()
    {
        // Arrange
        var command = new TestDbCommand();
        var parameters = new object[] { "foo", 42, 3.14 };

        // Act
        command.AddParameters(parameters);

        // Assert
        Assert.Equal(3, command.TestParameters.Count);
        Assert.Equal("foo", command.TestParameters.Parameters[0].Value);
        Assert.Equal(42, command.TestParameters.Parameters[1].Value);
        Assert.Equal(3.14, command.TestParameters.Parameters[2].Value);
    }

    [Fact]
    public void AddParameters_HandlesNullValues()
    {
        // Arrange
        var command = new TestDbCommand();
        var parameters = new object[] { null!, "bar" };

        // Act
        command.AddParameters(parameters);

        // Assert
        Assert.Equal(DBNull.Value, command.TestParameters.Parameters[0].Value);
        Assert.Equal("bar", command.TestParameters.Parameters[1].Value);
    }

    [Fact]
    public void AddParameters_HandlesEmptyArray()
    {
        // Arrange
        var command = new TestDbCommand();
        var parameters = Array.Empty<object>();

        // Act
        command.AddParameters(parameters);

        // Assert
        Assert.Empty(command.TestParameters.Parameters);
    }

    [Fact]
    public void AddParameters_HandlesMixedTypes()
    {
        // Arrange
        var command = new TestDbCommand();
        var dt = DateTime.UtcNow;
        var parameters = new object[] { 1, "x", null!, dt };

        // Act
        command.AddParameters(parameters);

        // Assert
        Assert.Equal(4, command.TestParameters.Count);
        Assert.Equal(1, command.TestParameters.Parameters[0].Value);
        Assert.Equal("x", command.TestParameters.Parameters[1].Value);
        Assert.Equal(DBNull.Value, command.TestParameters.Parameters[2].Value);
        Assert.Equal(dt, command.TestParameters.Parameters[3].Value);
    }

    [Fact]
    public void AddParameters_ThrowsIfCommandIsNull()
    {
        // Arrange
        DbCommand command = null!;
        var parameters = new object[] { 1 };

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => command.AddParameters(parameters));
    }

    [Fact]
    public void AddParameters_ThrowsIfParametersIsNull()
    {
        // Arrange
        var command = new TestDbCommand();
        object[] parameters = null!;

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => command.AddParameters(parameters));
    }

    [Fact]
    public void AddParameters_ParameterOrderIsPreserved()
    {
        // Arrange
        var command = new TestDbCommand();
        var parameters = new object[] { "first", "second", "third" };

        // Act
        command.AddParameters(parameters);

        // Assert
        Assert.Equal("first", command.TestParameters.Parameters[0].Value);
        Assert.Equal("second", command.TestParameters.Parameters[1].Value);
        Assert.Equal("third", command.TestParameters.Parameters[2].Value);
    }
}
