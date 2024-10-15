
### Using DateTimeHelper

The `DateTimeHelper` class provides flexible date parsing capabilities:
csharp
string dateString = "2023-04-15 14:30:00";
if (DateTimeHelper.TryParseDateTime(dateString, out DateTime result))
{
Console.WriteLine($"Parsed date: {result}");
}
// Using custom formats
string customDateString = "15/04/2023";
string[] customFormats = { "dd/MM/yyyy" };
if (DateTimeHelper.TryParseDateTime(customDateString, out DateTime customResult, customFormats))
{
Console.WriteLine($"Parsed custom date: {customResult}");
}

### Extending TypeConversionHelper

Add custom type mappings:
csharp
TypeConversionHelper.MergeCustomTypeMapping(new Dictionary<string, Type>
{
{ "custom_type", typeof(YourCustomType) }
});


## Extending TypeConversionHelper

#### Custom Type Converters

You can add custom type converters to handle specific type conversions:
csharp
// Define a custom type
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
// Register a custom converter
TypeConversionHelper.RegisterCustomConverter(typeof(CustomType), (value, targetType, customFormat) =>
{
var parts = value.ToString().Split(';');
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
// Usage
var rule = new DynamicRule
{
FieldName = "CustomField",
Operator = "equal",
Value = "1;John Doe;123 Main St;Anytown",
Type = "CustomType"
};
var result = TypeConversionHelper.ConvertValueToObjectArray(rule.Value, rule.Type);
// result will contain an array with a single CustomType object with Id = 1, Name = "John Doe", Street = "123 Main St", City = "Anytown"