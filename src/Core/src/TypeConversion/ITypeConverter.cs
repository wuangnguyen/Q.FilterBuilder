using System.Collections.Generic;

namespace Q.FilterBuilder.Core.TypeConversion;

/// <summary>
/// Defines a type converter for a specific target type.
/// </summary>
/// <typeparam name="T">The target type to convert to.</typeparam>
public interface ITypeConverter<T>
{
    /// <summary>
    /// Converts the specified value to the target type.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="metadata">Optional metadata for conversion context.</param>
    /// <returns>The converted value.</returns>
    T Convert(object? value, Dictionary<string, object?>? metadata = null);
}
