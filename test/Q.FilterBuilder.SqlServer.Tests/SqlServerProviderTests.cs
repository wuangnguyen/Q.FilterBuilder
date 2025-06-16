using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests;

public class SqlServerProviderTests
{
    private readonly SqlServerFormatProvider _provider;

    public SqlServerProviderTests()
    {
        _provider = new SqlServerFormatProvider();
    }

    [Fact]
    public void ParameterPrefix_ShouldReturnAtSymbol()
    {
        // Act
        var result = _provider.ParameterPrefix;

        // Assert
        Assert.Equal("@", result);
    }

    [Fact]
    public void AndOperator_ShouldReturnAND()
    {
        // Act
        var result = _provider.AndOperator;

        // Assert
        Assert.Equal("AND", result);
    }

    [Fact]
    public void OrOperator_ShouldReturnOR()
    {
        // Act
        var result = _provider.OrOperator;

        // Assert
        Assert.Equal("OR", result);
    }

    [Fact]
    public void FormatFieldName_ShouldWrapInSquareBrackets()
    {
        // Arrange
        var fieldName = "UserName";

        // Act
        var result = _provider.FormatFieldName(fieldName);

        // Assert
        Assert.Equal("[UserName]", result);
    }

    [Fact]
    public void FormatFieldName_WithSpecialCharacters_ShouldWrapInSquareBrackets()
    {
        // Arrange
        var fieldName = "User Name";

        // Act
        var result = _provider.FormatFieldName(fieldName);

        // Assert
        Assert.Equal("[User Name]", result);
    }

    [Fact]
    public void FormatFieldName_WithEmptyString_ShouldWrapInSquareBrackets()
    {
        // Arrange
        var fieldName = "";

        // Act
        var result = _provider.FormatFieldName(fieldName);

        // Assert
        Assert.Equal("[]", result);
    }

    [Fact]
    public void FormatFieldName_WithNullString_ShouldWrapInSquareBrackets()
    {
        // Arrange
        string fieldName = null!;

        // Act
        var result = _provider.FormatFieldName(fieldName);

        // Assert
        Assert.Equal("[]", result);
    }

    [Fact]
    public void FormatFieldName_WithWhitespaceString_ShouldWrapInSquareBrackets()
    {
        // Arrange
        var fieldName = "   ";

        // Act
        var result = _provider.FormatFieldName(fieldName);

        // Assert
        Assert.Equal("[   ]", result);
    }

    [Fact]
    public void FormatFieldName_WithUnicodeCharacters_ShouldWrapInSquareBrackets()
    {
        // Arrange
        var fieldName = "用户名";

        // Act
        var result = _provider.FormatFieldName(fieldName);

        // Assert
        Assert.Equal("[用户名]", result);
    }

    [Fact]
    public void FormatFieldName_WithSpecialSqlCharacters_ShouldWrapInSquareBrackets()
    {
        // Arrange
        var fieldName = "User[Name]";

        // Act
        var result = _provider.FormatFieldName(fieldName);

        // Assert
        Assert.Equal("[User[Name]]", result);
    }

    [Theory]
    [InlineData(0, "@p0")]
    [InlineData(1, "@p1")]
    [InlineData(10, "@p10")]
    [InlineData(100, "@p100")]
    [InlineData(1000, "@p1000")]
    public void FormatParameterName_WithVariousIndices_ShouldReturnCorrectFormat(int index, string expected)
    {
        // Act
        var result = _provider.FormatParameterName(index);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Name", "[Name]")]
    [InlineData("User_Name", "[User_Name]")]
    [InlineData("User-Name", "[User-Name]")]
    [InlineData("User.Name", "[User.Name]")]
    [InlineData("User@Name", "[User@Name]")]
    [InlineData("User#Name", "[User#Name]")]
    [InlineData("User$Name", "[User$Name]")]
    [InlineData("User%Name", "[User%Name]")]
    [InlineData("User^Name", "[User^Name]")]
    [InlineData("User&Name", "[User&Name]")]
    [InlineData("User*Name", "[User*Name]")]
    [InlineData("User(Name)", "[User(Name)]")]
    [InlineData("User+Name", "[User+Name]")]
    [InlineData("User=Name", "[User=Name]")]
    [InlineData("User{Name}", "[User{Name}]")]
    [InlineData("User|Name", "[User|Name]")]
    [InlineData("User\\Name", "[User\\Name]")]
    [InlineData("User:Name", "[User:Name]")]
    [InlineData("User;Name", "[User;Name]")]
    [InlineData("User\"Name", "[User\"Name]")]
    [InlineData("User'Name", "[User'Name]")]
    [InlineData("User<Name>", "[User<Name>]")]
    [InlineData("User,Name", "[User,Name]")]
    [InlineData("User?Name", "[User?Name]")]
    [InlineData("User/Name", "[User/Name]")]
    [InlineData("User~Name", "[User~Name]")]
    [InlineData("User`Name", "[User`Name]")]
    public void FormatFieldName_WithVariousSpecialCharacters_ShouldWrapInSquareBrackets(string fieldName, string expected)
    {
        // Act
        var result = _provider.FormatFieldName(fieldName);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Properties_ShouldBeConsistent()
    {
        // Act & Assert - Test that properties return consistent values
        Assert.Equal("@", _provider.ParameterPrefix);
        Assert.Equal("AND", _provider.AndOperator);
        Assert.Equal("OR", _provider.OrOperator);

        // Test consistency across multiple calls
        Assert.Equal(_provider.ParameterPrefix, _provider.ParameterPrefix);
        Assert.Equal(_provider.AndOperator, _provider.AndOperator);
        Assert.Equal(_provider.OrOperator, _provider.OrOperator);
    }

    [Fact]
    public void Provider_ShouldImplementIQueryFormatProvider()
    {
        // Assert
        Assert.IsAssignableFrom<Core.Providers.IQueryFormatProvider>(_provider);
    }

    [Fact]
    public void FormatFieldName_WithTabsAndNewlines_ShouldWrapInSquareBrackets()
    {
        // Arrange
        var fieldName = "User\tName\nField";

        // Act
        var result = _provider.FormatFieldName(fieldName);

        // Assert
        Assert.Equal("[User\tName\nField]", result);
    }

    [Fact]
    public void FormatFieldName_WithNumericString_ShouldWrapInSquareBrackets()
    {
        // Arrange
        var fieldName = "123456";

        // Act
        var result = _provider.FormatFieldName(fieldName);

        // Assert
        Assert.Equal("[123456]", result);
    }
}
