using System.Collections.Generic;

namespace Q.FilterBuilder.Core.TypeConversion;

/// <summary>
/// Provides type conversion services for FilterBuilder.
/// </summary>
public interface ITypeConversionService
{
    /// <summary>
    /// Converts a value to the specified type.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="typeString">The target type string (e.g., "int", "datetime", "string").</param>
    /// <param name="metadata">Optional metadata for conversion context.</param>
    /// <returns>The converted value, or the original value if no converter is found.</returns>
    object? ConvertValue(object? value, string typeString, Dictionary<string, object?>? metadata = null);

    /// <summary>
    /// Registers a type converter for a specific type string.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="typeString">The type string identifier.</param>
    /// <param name="converter">The converter instance.</param>
    void RegisterConverter<T>(string typeString, ITypeConverter<T> converter);

    /// <summary>
    /// Determines if the given value is a collection (array, list, etc.) but not a string.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>true if the value is a collection; otherwise, false.</returns>
    bool IsCollection(object? value);
}
