using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DynamicWhere.Core.Helpers;

/// <summary>
/// Provides helper methods for type conversion operations.
/// </summary>
public static class TypeConversionHelper
{
    private static readonly Dictionary<string, Type> GlobalTypeMapping = new()
    {
        { "string", typeof(string) },
        { "int", typeof(int) },
        { "integer", typeof(int) },
        { "double", typeof(double) },
        { "bool", typeof(bool) },
        { "boolean", typeof(bool) },
        { "datetime", typeof(DateTime) },
        { "date", typeof(DateTime) }
    };

    private static readonly Dictionary<Type, CustomTypeConverter> CustomConverters = new();

    /// <summary>
    /// Merges custom type mappings into the global type mapping dictionary.
    /// </summary>
    /// <param name="customMappings">A dictionary of custom type mappings to merge.</param>
    public static void MergeCustomTypeMapping(Dictionary<string, Type> customMappings)
    {
        foreach (var mapping in customMappings)
        {
            GlobalTypeMapping[mapping.Key] = mapping.Value;
        }
    }

    /// <summary>
    /// Delegate for custom type conversion.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="targetType">The target type for conversion.</param>
    /// <returns>The converted value.</returns>
    public delegate object CustomTypeConverter(object value, Type targetType);

    /// <summary>
    /// Registers a custom type converter for a specific type.
    /// </summary>
    /// <param name="type">The type for which the custom converter is registered.</param>
    /// <param name="converter">The custom type converter to register.</param>
    public static void RegisterCustomConverter(Type type, CustomTypeConverter converter)
    {
        CustomConverters[type] = converter;
    }

    /// <summary>
    /// Converts a value to an array of objects based on the specified type.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="type">The target type for conversion.</param>
    /// <param name="customFormat">Optional. A custom format string for date/time parsing.</param>
    /// <returns>An array of converted objects, or null if the input value is null.</returns>
    public static object[]? ConvertValueToObjectArray(object? value, string type, string? customFormat = null)
    {
        if (value == null)
        {
            return null;
        }

        Type targetType = DetermineTargetType(value, type);

        if (IsCollection(value))
        {
            var elementType = targetType.IsArray ? targetType.GetElementType()! : targetType;
            return ConvertCollection(value, elementType, customFormat);
        }

        return [ConvertSingleValue(value, targetType, customFormat)];
    }

    private static Type DetermineTargetType(object value, string type)
    {
        if (string.IsNullOrEmpty(type))
        {
            return value.GetType();
        }

        if (!GlobalTypeMapping.TryGetValue(type, out Type? mappedType))
        {
            mappedType = Type.GetType(type, throwOnError: true, ignoreCase: true)!;
        }

        return mappedType;
    }

    private static object[] ConvertCollection(object value, Type elementType, string? customFormat)
    {
        if (value.GetType().GetElementType() == elementType)
        {
            return value switch
            {
                Array array => array.Cast<object>().ToArray(),
                IEnumerable<object> enumerable => enumerable.ToArray(),
                _ => throw new ArgumentException("Unsupported collection type.")
            };
        }

        return (value as IEnumerable)!
            .Cast<object>().Select(v => ConvertSingleValue(v, elementType, customFormat))
            .ToArray();
    }

    private static object ConvertSingleValue(object value, Type targetType, string? customFormat)
    {
        if (value.GetType() == targetType)
        {
            return value;
        }

        if (CustomConverters.TryGetValue(targetType, out var customConverter))
        {
            return customConverter(value, targetType);
        }

        if (targetType == typeof(DateTime) && 
            DateTimeHelper.TryParseDateTime(value.ToString()!, out var parsedDateTime, customFormat!))
        {
            return parsedDateTime;
        }

        return Convert.ChangeType(value, targetType);
    }

    /// <summary>
    /// Determines if the given value is a collection.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>true if the value is a collection; otherwise, false.</returns>
    public static bool IsCollection(object? value)
    {
        if (value == null)
        {
            return false;
        }

        var type = value.GetType();

        return type.IsArray || (value is IEnumerable && value is not string);
    }
}