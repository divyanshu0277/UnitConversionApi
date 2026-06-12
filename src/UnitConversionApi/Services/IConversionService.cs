using UnitConversionApi.Models;

namespace UnitConversionApi.Services;

/// <summary>
/// Defines the contract for performing unit conversions and querying supported units.
/// </summary>
public interface IConversionService
{
    /// <summary>
    /// Converts a value from one unit to another.
    /// </summary>
    /// <param name="request">The conversion parameters.</param>
    /// <returns>
    /// A <see cref="ConversionResult"/> on success, or a <see cref="ConversionError"/>
    /// describing why the conversion could not be performed.
    /// </returns>
    OneOf<ConversionResult, ConversionError> Convert(ConversionRequest request);

    /// <summary>Returns all units supported by the API, optionally filtered by category.</summary>
    IReadOnlyList<UnitInfo> GetSupportedUnits(string? category = null);

    /// <summary>Returns all supported category names.</summary>
    IReadOnlyList<string> GetCategories();
}
