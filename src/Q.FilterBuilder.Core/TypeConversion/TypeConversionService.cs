using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Q.FilterBuilder.Core.TypeConversion;

/// <summary>
/// Default implementation of ITypeConversionService.
/// </summary>
public class TypeConversionService : ITypeConversionService
{
    private readonly Dictionary<string, object> _converters = new();

    /// <summary>
    /// Initializes a new instance of the TypeConversionService class.
    /// </summary>
    public TypeConversionService()
    {
        RegisterBuiltInConverters();
    }

    /// <inheritdoc />
    public object? ConvertValue(object? value, string typeString, Dictionary<string, object?>? metadata = null)
    {
        if (value == null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(typeString))
        {
            return value;
        }

        // Handle collections (arrays, lists, etc.) - convert each element
        if (IsCollection(value))
        {
            return ConvertCollection(value, typeString, metadata);
        }

        // Handle single values
        return ConvertSingleValue(value, typeString, metadata);
    }

    /// <inheritdoc />
    public void RegisterConverter<T>(string typeString, ITypeConverter<T> converter)
    {
        if (string.IsNullOrWhiteSpace(typeString))
        {
            throw new ArgumentException("Type string cannot be null or empty.", nameof(typeString));
        }

        if (converter == null)
        {
            throw new ArgumentNullException(nameof(converter));
        }

        _converters[typeString] = converter;
    }

    /// <inheritdoc />
    public bool IsCollection(object? value)
    {
        if (value == null)
        {
            return false;
        }

        var type = value.GetType();

        return type.IsArray || (value is IEnumerable && value is not string);
    }

    private void RegisterBuiltInConverters()
    {
        // Register custom converters for complex types
        RegisterConverter("datetime", new DateTimeTypeConverter());
        RegisterConverter("date", new DateTimeTypeConverter());
        RegisterConverter("bool", new BoolTypeConverter());
        RegisterConverter("boolean", new BoolTypeConverter());

        // Primitive types use default conversion (no custom converters needed)
        // "int", "integer", "double", "decimal", "string" will use TryDefaultConvert method
    }

    /// <summary>
    /// Converts a collection by converting each element to the specified type.
    /// </summary>
    /// <param name="value">The collection to convert.</param>
    /// <param name="typeString">The target type string for each element.</param>
    /// <param name="metadata">Optional metadata for conversion context.</param>
    /// <returns>A strongly-typed array of converted objects.</returns>
    private object ConvertCollection(object value, string typeString, Dictionary<string, object?>? metadata)
    {
        try
        {
            var convertedItems = ((IEnumerable)value)
                .Cast<object>()
                .Select(item => ConvertSingleValue(item, typeString, metadata) ?? item)
                .ToArray();

            // Create strongly-typed array based on the target type
            var targetType = GetTargetType(typeString);
            if (targetType != null)
            {
                var typedArray = Array.CreateInstance(targetType, convertedItems.Length);
                for (var i = 0; i < convertedItems.Length; i++)
                {
                    typedArray.SetValue(convertedItems[i], i);
                }
                return typedArray;
            }

            // Fallback to object array if type is not recognized
            return convertedItems;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to convert collection to type '{typeString}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Converts a single value to the specified type.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="typeString">The target type string.</param>
    /// <param name="metadata">Optional metadata for conversion context.</param>
    /// <returns>The converted value.</returns>
    private object? ConvertSingleValue(object? value, string typeString, Dictionary<string, object?>? metadata)
    {
        if (value == null)
        {
            return null;
        }

        // Try custom converter first
        if (!_converters.TryGetValue(typeString, out var converter))
        {
            // No custom converter found, try default conversion
            return TryDefaultConvert(value, typeString);
        }

        try
        {
            // Call Convert method on the converter using reflection
            var convertMethod = converter.GetType().GetMethod("Convert")!;
            return convertMethod.Invoke(converter, [value, metadata]);
        }
        catch (Exception ex)
        {
            // Custom converter failed - rethrow with context
            throw new InvalidOperationException($"Failed to convert '{value}' to type '{typeString}' using custom converter: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Attempts to convert a value using .NET's built-in conversion for primitive types.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="typeString">The target type string.</param>
    /// <returns>The converted value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when conversion fails.</exception>
    private object? TryDefaultConvert(object? value, string typeString)
    {
        if (value == null)
        {
            return null;
        }

        try
        {
            return typeString.ToLowerInvariant() switch
            {
                "string" => value.ToString(),
                "int" or "integer" => Convert.ToInt32(value, CultureInfo.InvariantCulture),
                "long" => Convert.ToInt64(value, CultureInfo.InvariantCulture),
                "double" => Convert.ToDouble(value, CultureInfo.InvariantCulture),
                "decimal" => Convert.ToDecimal(value, CultureInfo.InvariantCulture),
                "float" => Convert.ToSingle(value, CultureInfo.InvariantCulture),
                "byte" => Convert.ToByte(value, CultureInfo.InvariantCulture),
                "short" => Convert.ToInt16(value, CultureInfo.InvariantCulture),
                "uint" => Convert.ToUInt32(value, CultureInfo.InvariantCulture),
                "ulong" => Convert.ToUInt64(value, CultureInfo.InvariantCulture),
                "ushort" => Convert.ToUInt16(value, CultureInfo.InvariantCulture),
                "sbyte" => Convert.ToSByte(value, CultureInfo.InvariantCulture),
                "guid" => value is string str ? Guid.Parse(str) : (Guid)value,
                _ => throw new InvalidOperationException($"Unsupported type conversion: cannot convert '{value}' to type '{typeString}'")
            };
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new InvalidOperationException($"Failed to convert '{value}' to type '{typeString}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets the target .NET type for a given type string.
    /// </summary>
    /// <param name="typeString">The type string.</param>
    /// <returns>The corresponding .NET type, or null if not found.</returns>
    private Type? GetTargetType(string typeString)
    {
        // First check if there's a custom converter registered for this type
        if (_converters.TryGetValue(typeString, out var converter))
        {
            // Extract the target type from the ITypeConverter<T> interface
            var converterType = converter.GetType();
            var interfaceType = converterType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITypeConverter<>));

            if (interfaceType != null)
            {
                return interfaceType.GetGenericArguments()[0];
            }
        }

        // Fallback to built-in types
        return typeString.ToLowerInvariant() switch
        {
            "string" => typeof(string),
            "int" or "integer" => typeof(int),
            "long" => typeof(long),
            "double" => typeof(double),
            "decimal" => typeof(decimal),
            "float" => typeof(float),
            "byte" => typeof(byte),
            "short" => typeof(short),
            "uint" => typeof(uint),
            "ulong" => typeof(ulong),
            "ushort" => typeof(ushort),
            "sbyte" => typeof(sbyte),
            "bool" or "boolean" => typeof(bool),
            "datetime" or "date" => typeof(DateTime),
            "guid" => typeof(Guid),
            _ => null
        };
    }
}

