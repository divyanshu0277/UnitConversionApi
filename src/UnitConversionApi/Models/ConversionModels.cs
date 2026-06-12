namespace UnitConversionApi.Models;

/// <summary>
/// Request payload for a unit conversion operation.
/// </summary>
public record ConversionRequest
{
    /// <summary>
    /// The numeric value to convert.
    /// </summary>
    /// <example>100</example>
    public required double Value { get; init; }

    /// <summary>
    /// The unit to convert from (e.g., "meters", "celsius", "kilograms").
    /// </summary>
    /// <example>celsius</example>
    public required string FromUnit { get; init; }

    /// <summary>
    /// The unit to convert to (e.g., "feet", "fahrenheit", "pounds").
    /// </summary>
    /// <example>fahrenheit</example>
    public required string ToUnit { get; init; }
}

/// <summary>
/// The result of a unit conversion operation.
/// </summary>
public record ConversionResult
{
    /// <summary>The original input value.</summary>
    public required double InputValue { get; init; }

    /// <summary>The unit that was converted from.</summary>
    public required string FromUnit { get; init; }

    /// <summary>The unit that was converted to.</summary>
    public required string ToUnit { get; init; }

    /// <summary>The converted output value.</summary>
    public required double OutputValue { get; init; }

    /// <summary>The conversion category (e.g., Length, Temperature, Weight).</summary>
    public required string Category { get; init; }
}

/// <summary>
/// Error response returned when a conversion cannot be performed.
/// </summary>
public record ConversionError
{
    /// <summary>Human-readable error message.</summary>
    public required string Message { get; init; }

    /// <summary>Optional details about why the error occurred.</summary>
    public string? Detail { get; init; }
}

/// <summary>
/// Describes a supported unit.
/// </summary>
public record UnitInfo
{
    /// <summary>The canonical key used in API requests (e.g., "meters").</summary>
    public required string Key { get; init; }

    /// <summary>Human-readable display name.</summary>
    public required string DisplayName { get; init; }

    /// <summary>Common aliases accepted by the API.</summary>
    public required IReadOnlyList<string> Aliases { get; init; }

    /// <summary>The conversion category this unit belongs to.</summary>
    public required string Category { get; init; }
}
