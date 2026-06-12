using UnitConversionApi.Configuration;
using UnitConversionApi.Models;

namespace UnitConversionApi.Services;

/// <summary>
/// Performs unit conversions using a two-step base-unit approach:
///   1. Convert the input value from the source unit to the category's base unit.
///   2. Convert from the base unit to the target unit.
///
/// This strategy requires only one conversion definition per unit (vs. base),
/// making it easy to add new units without touching existing conversion logic.
/// </summary>
public sealed class ConversionService : IConversionService
{
    /// <inheritdoc/>
    public OneOf<ConversionResult, ConversionError> Convert(ConversionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FromUnit))
            return new ConversionError { Message = "fromUnit is required." };

        if (string.IsNullOrWhiteSpace(request.ToUnit))
            return new ConversionError { Message = "toUnit is required." };

        if (!TryResolveUnit(request.FromUnit, out var fromDef))
            return new ConversionError
            {
                Message = $"Unknown unit: '{request.FromUnit}'.",
                Detail = "Use GET /units to list all supported units and their aliases."
            };

        if (!TryResolveUnit(request.ToUnit, out var toDef))
            return new ConversionError
            {
                Message = $"Unknown unit: '{request.ToUnit}'.",
                Detail = "Use GET /units to list all supported units and their aliases."
            };

        if (!string.Equals(fromDef.Category, toDef.Category, StringComparison.OrdinalIgnoreCase))
            return new ConversionError
            {
                Message = $"Cannot convert between '{fromDef.DisplayName}' ({fromDef.Category}) " +
                          $"and '{toDef.DisplayName}' ({toDef.Category}).",
                Detail = "Units must belong to the same category (e.g., both Length or both Temperature)."
            };

        // Two-step conversion via base unit
        var baseValue = fromDef.ToBase(request.Value);
        var outputValue = toDef.FromBase(baseValue);

        return new ConversionResult
        {
            InputValue = request.Value,
            FromUnit = fromDef.DisplayName,
            ToUnit = toDef.DisplayName,
            OutputValue = Math.Round(outputValue, 10),
            Category = fromDef.Category
        };
    }

    /// <inheritdoc/>
    public IReadOnlyList<UnitInfo> GetSupportedUnits(string? category = null)
    {
        var units = UnitRegistry.AllUnits.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(category))
            units = units.Where(u => u.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

        return units
            .OrderBy(u => u.Category)
            .ThenBy(u => u.DisplayName)
            .Select(u => new UnitInfo
            {
                Key = u.Key,
                DisplayName = u.DisplayName,
                Category = u.Category,
                Aliases = u.Aliases
            })
            .ToList();
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> GetCategories() => UnitRegistry.Categories;

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static bool TryResolveUnit(string input, out UnitDefinition definition)
    {
        return UnitRegistry.AliasIndex.TryGetValue(input.Trim(), out definition!);
    }
}
