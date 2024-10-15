```
public class CustomDateTimeParser : IValueParserStrategy
{
    public object? ParseValue(JsonElement element)
    {
        var stringValue = element.GetString();
        return stringValue switch
        {
            "today()" => DateTime.Today,
            "now()" => DateTime.Now,
            _ => DateTime.Parse(stringValue!)
        };
    }
}
```

```
var customParsers = new Dictionary<string, IValueParserStrategy>
{
    { "datetime", new CustomDateTimeParser() }
};

var options = new JsonSerializerOptions
{
    Converters = { new JQueryBuilderConverter(customParsers) }
};

string json = "{...}"; // Your JSON content here
var dynamicGroup = JsonSerializer.Deserialize<DynamicGroup>(json, options);
```

```
string[] defaultFormats =
    [
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-dd HH:mm",
        "yyyy-MM-dd",
        "MM/dd/yyyy",
        "MM/dd/yyyy HH:mm",
        "MM/dd/yyyy HH:mm:ss",
        "dd-MM-yyyy",
        "dd-MM-yyyy HH:mm",
        "dd-MM-yyyy HH:mm:ss",
        "yyyyMMddTHHmmssZ",  // ISO 8601 format
        "yyyy-MM-ddTHH:mm:ssZ"  // ISO 8601 format with T
    ];
```