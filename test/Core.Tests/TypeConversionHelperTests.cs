using Shouldly;
using DynamicWhere.Core.Helpers;
using System.Net;

namespace Core.Tests;

public class TypeConversionHelperTests
{
    [Fact]
    public void ConvertValueToObjectArray_ShouldConvertStringToInt()
    {
        var result = TypeConversionHelper.ConvertValueToObjectArray("42", "int");
        result.ShouldBe([42]);
    }

    [Fact]
    public void ConvertValueToObjectArray_ShouldConvertStringArrayToIntArray()
    {
        var result = TypeConversionHelper.ConvertValueToObjectArray(new[] { "1", "2", "3" }, "int");
        result.ShouldBeOfType<object[]>().ShouldBe([1, 2, 3]);
    }

    [Fact]
    public void ConvertValueToObjectArray_ShouldConvertStringArrayToIntArray1()
    {
        var result = TypeConversionHelper.ConvertValueToObjectArray(new[] { "1", "2", "3" }, "string");
        result.ShouldBeOfType<object[]>().ShouldBe(["1", "2", "3"]);
    }

    [Fact]
    public void ConvertValueToObjectArray_ShouldConvertStringArrayToDateTimeArray()
    {
        var result = TypeConversionHelper.ConvertValueToObjectArray(new[] { "2024-08-15", "2024-08-20" }, "datetime");
        result.ShouldBeOfType<object[]>().ShouldBe(
        [
            new DateTime(2024, 08, 15),
            new DateTime(2024, 08, 20)
        ]);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("")]
    public void ConvertValueToObjectArray_ShouldThrowExceptionForInvalidValue(string value)
    {
        Should.Throw<Exception>(() =>
        {
            TypeConversionHelper.ConvertValueToObjectArray(value, "int");
        });
    }

    [Fact]
    public void IsCollection_ShouldReturnsTrue()
    {
        var result1 = TypeConversionHelper.IsCollection(new int[] { 1, 2, 3 });
        var result2 = TypeConversionHelper.IsCollection(new List<int> { 1, 2, 3 });
        var result3 = TypeConversionHelper.IsCollection(new string[] { "test1", "test2" });

        result1.ShouldBeTrue();
        result2.ShouldBeTrue();
        result3.ShouldBeTrue();
    }

    [Fact]
    public void IsCollection_ShouldReturnsFalse()
    {
        var result1 = TypeConversionHelper.IsCollection("This is a string");
        var result2 = TypeConversionHelper.IsCollection(null!);

        result1.ShouldBeFalse();
        result2.ShouldBeFalse();
    }

    [Fact]
    public void ConvertValueToObjectArray_CustomType_Success()
    {
        // Arrange
        TypeConversionHelper.RegisterCustomConverter(typeof(CustomType), (value, targetType) =>
        {
            var parts = value.ToString()!.Split(';');
            return new CustomType
            {
                Id = int.Parse(parts[0]),
                Name = parts[1],
                Address = new Address
                {
                    Street = parts[2],
                    City = parts[3]
                }
            };
        });
        var value = "1;John Doe;123 Main St;Anytown";
        var type = "CustomType";

        // Act
        var result = TypeConversionHelper.ConvertValueToObjectArray(value, type);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var customType = Assert.IsType<CustomType>(result[0]);
        Assert.Equal(1, customType.Id);
        Assert.Equal("John Doe", customType.Name);
        Assert.Equal("123 Main St", customType.Address.Street);
        Assert.Equal("Anytown", customType.Address.City);
    }
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
}

public class CustomType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }
}