using System.Collections.Generic;
using System.Globalization;

namespace Q.FilterBuilder.Core.TypeConversion;

/// <summary>
/// Converts values to bool with support for common string representations.
/// </summary>
public class BoolTypeConverter : ITypeConverter<bool>
{
    /// <inheritdoc />
    public bool Convert(object? value, Dictionary<string, object?>? metadata = null)
    {
        if (value is bool boolValue)
        {
            return boolValue;
        }

        if (value == null)
        {
            return false;
        }

        var stringValue = value.ToString()!.ToLowerInvariant();

        return stringValue switch
        {
            "true" or "1" or "yes" or "on" => true,
            "false" or "0" or "no" or "off" => false,
            _ => System.Convert.ToBoolean(value, CultureInfo.InvariantCulture)
        };
    }
}
