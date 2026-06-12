namespace UnitConversionApi.Configuration;

/// <summary>
/// Represents a single unit of measurement and its conversion relationship
/// relative to a canonical base unit within its category.
/// 
/// Design: Each category uses a single base unit (e.g., meters for length).
/// All conversions go: fromUnit → baseUnit → toUnit, using the stored
/// ToBase / FromBase delegates. Temperature uses custom delegates to handle
/// non-linear offsets; all other units use simple linear scale factors.
/// 
/// This approach makes adding new units trivial — no need to define N² 
/// pairwise conversion functions, just one base-relative relationship per unit.
/// </summary>
public record UnitDefinition
{
    public required string Key { get; init; }
    public required string DisplayName { get; init; }
    public required string Category { get; init; }
    public required IReadOnlyList<string> Aliases { get; init; }

    /// <summary>Converts a value in this unit to the category's base unit.</summary>
    public required Func<double, double> ToBase { get; init; }

    /// <summary>Converts a value from the category's base unit to this unit.</summary>
    public required Func<double, double> FromBase { get; init; }
}

/// <summary>
/// Provides the full registry of supported units, grouped by category.
/// 
/// Extending the registry: add a new UnitDefinition to the relevant list,
/// or add a new category list entirely. The conversion engine requires no changes.
/// </summary>
public static class UnitRegistry
{
    // ── Length (base: meters) ─────────────────────────────────────────────────

    private static readonly List<UnitDefinition> LengthUnits =
    [
        new()
        {
            Key = "meters", DisplayName = "Meters", Category = "Length",
            Aliases = ["meter", "m", "metre", "metres"],
            ToBase = v => v,
            FromBase = v => v
        },
        new()
        {
            Key = "kilometers", DisplayName = "Kilometers", Category = "Length",
            Aliases = ["kilometer", "km", "kilometre", "kilometres"],
            ToBase = v => v * 1000,
            FromBase = v => v / 1000
        },
        new()
        {
            Key = "centimeters", DisplayName = "Centimeters", Category = "Length",
            Aliases = ["centimeter", "cm", "centimetre", "centimetres"],
            ToBase = v => v / 100,
            FromBase = v => v * 100
        },
        new()
        {
            Key = "millimeters", DisplayName = "Millimeters", Category = "Length",
            Aliases = ["millimeter", "mm", "millimetre", "millimetres"],
            ToBase = v => v / 1000,
            FromBase = v => v * 1000
        },
        new()
        {
            Key = "miles", DisplayName = "Miles", Category = "Length",
            Aliases = ["mile", "mi"],
            ToBase = v => v * 1609.344,
            FromBase = v => v / 1609.344
        },
        new()
        {
            Key = "yards", DisplayName = "Yards", Category = "Length",
            Aliases = ["yard", "yd"],
            ToBase = v => v * 0.9144,
            FromBase = v => v / 0.9144
        },
        new()
        {
            Key = "feet", DisplayName = "Feet", Category = "Length",
            Aliases = ["foot", "ft"],
            ToBase = v => v * 0.3048,
            FromBase = v => v / 0.3048
        },
        new()
        {
            Key = "inches", DisplayName = "Inches", Category = "Length",
            Aliases = ["inch", "in"],
            ToBase = v => v * 0.0254,
            FromBase = v => v / 0.0254
        },
        new()
        {
            Key = "nautical-miles", DisplayName = "Nautical Miles", Category = "Length",
            Aliases = ["nautical mile", "nmi", "nm"],
            ToBase = v => v * 1852,
            FromBase = v => v / 1852
        },
    ];

    // ── Temperature (base: Celsius) ───────────────────────────────────────────

    private static readonly List<UnitDefinition> TemperatureUnits =
    [
        new()
        {
            Key = "celsius", DisplayName = "Celsius", Category = "Temperature",
            Aliases = ["°c", "c", "centigrade"],
            ToBase = v => v,
            FromBase = v => v
        },
        new()
        {
            Key = "fahrenheit", DisplayName = "Fahrenheit", Category = "Temperature",
            Aliases = ["°f", "f"],
            ToBase = v => (v - 32) * 5 / 9,
            FromBase = v => v * 9 / 5 + 32
        },
        new()
        {
            Key = "kelvin", DisplayName = "Kelvin", Category = "Temperature",
            Aliases = ["k"],
            ToBase = v => v - 273.15,
            FromBase = v => v + 273.15
        },
        new()
        {
            Key = "rankine", DisplayName = "Rankine", Category = "Temperature",
            Aliases = ["°r", "r"],
            ToBase = v => (v - 491.67) * 5 / 9,
            FromBase = v => (v + 273.15) * 9 / 5
        },
    ];

    // ── Weight / Mass (base: kilograms) ──────────────────────────────────────

    private static readonly List<UnitDefinition> WeightUnits =
    [
        new()
        {
            Key = "kilograms", DisplayName = "Kilograms", Category = "Weight",
            Aliases = ["kilogram", "kg", "kgs"],
            ToBase = v => v,
            FromBase = v => v
        },
        new()
        {
            Key = "grams", DisplayName = "Grams", Category = "Weight",
            Aliases = ["gram", "g"],
            ToBase = v => v / 1000,
            FromBase = v => v * 1000
        },
        new()
        {
            Key = "milligrams", DisplayName = "Milligrams", Category = "Weight",
            Aliases = ["milligram", "mg"],
            ToBase = v => v / 1_000_000,
            FromBase = v => v * 1_000_000
        },
        new()
        {
            Key = "pounds", DisplayName = "Pounds", Category = "Weight",
            Aliases = ["pound", "lb", "lbs"],
            ToBase = v => v * 0.45359237,
            FromBase = v => v / 0.45359237
        },
        new()
        {
            Key = "ounces", DisplayName = "Ounces", Category = "Weight",
            Aliases = ["ounce", "oz"],
            ToBase = v => v * 0.028349523125,
            FromBase = v => v / 0.028349523125
        },
        new()
        {
            Key = "metric-tons", DisplayName = "Metric Tons", Category = "Weight",
            Aliases = ["metric ton", "tonne", "tonnes", "t"],
            ToBase = v => v * 1000,
            FromBase = v => v / 1000
        },
        new()
        {
            Key = "short-tons", DisplayName = "Short Tons (US)", Category = "Weight",
            Aliases = ["short ton", "ton", "tons", "us ton"],
            ToBase = v => v * 907.18474,
            FromBase = v => v / 907.18474
        },
        new()
        {
            Key = "stone", DisplayName = "Stone", Category = "Weight",
            Aliases = ["stones", "st"],
            ToBase = v => v * 6.35029318,
            FromBase = v => v / 6.35029318
        },
    ];

    // ── Volume (base: liters) ─────────────────────────────────────────────────

    private static readonly List<UnitDefinition> VolumeUnits =
    [
        new()
        {
            Key = "liters", DisplayName = "Liters", Category = "Volume",
            Aliases = ["liter", "l", "litre", "litres"],
            ToBase = v => v,
            FromBase = v => v
        },
        new()
        {
            Key = "milliliters", DisplayName = "Milliliters", Category = "Volume",
            Aliases = ["milliliter", "ml", "millilitre", "millilitres"],
            ToBase = v => v / 1000,
            FromBase = v => v * 1000
        },
        new()
        {
            Key = "cubic-meters", DisplayName = "Cubic Meters", Category = "Volume",
            Aliases = ["cubic meter", "m³", "m3"],
            ToBase = v => v * 1000,
            FromBase = v => v / 1000
        },
        new()
        {
            Key = "gallons-us", DisplayName = "Gallons (US)", Category = "Volume",
            Aliases = ["gallon", "gallons", "gal", "us gallon"],
            ToBase = v => v * 3.785411784,
            FromBase = v => v / 3.785411784
        },
        new()
        {
            Key = "gallons-uk", DisplayName = "Gallons (UK/Imperial)", Category = "Volume",
            Aliases = ["imperial gallon", "uk gallon", "imp gal"],
            ToBase = v => v * 4.54609,
            FromBase = v => v / 4.54609
        },
        new()
        {
            Key = "fluid-ounces", DisplayName = "Fluid Ounces (US)", Category = "Volume",
            Aliases = ["fluid ounce", "fl oz", "floz"],
            ToBase = v => v * 0.0295735296,
            FromBase = v => v / 0.0295735296
        },
        new()
        {
            Key = "cups", DisplayName = "Cups (US)", Category = "Volume",
            Aliases = ["cup"],
            ToBase = v => v * 0.2365882365,
            FromBase = v => v / 0.2365882365
        },
        new()
        {
            Key = "pints", DisplayName = "Pints (US)", Category = "Volume",
            Aliases = ["pint", "pt"],
            ToBase = v => v * 0.473176473,
            FromBase = v => v / 0.473176473
        },
        new()
        {
            Key = "quarts", DisplayName = "Quarts (US)", Category = "Volume",
            Aliases = ["quart", "qt"],
            ToBase = v => v * 0.946352946,
            FromBase = v => v / 0.946352946
        },
    ];

    // ── Speed (base: meters per second) ──────────────────────────────────────

    private static readonly List<UnitDefinition> SpeedUnits =
    [
        new()
        {
            Key = "meters-per-second", DisplayName = "Meters per Second", Category = "Speed",
            Aliases = ["m/s", "mps", "meter per second"],
            ToBase = v => v,
            FromBase = v => v
        },
        new()
        {
            Key = "kilometers-per-hour", DisplayName = "Kilometers per Hour", Category = "Speed",
            Aliases = ["km/h", "kph", "kmh", "kilometer per hour"],
            ToBase = v => v / 3.6,
            FromBase = v => v * 3.6
        },
        new()
        {
            Key = "miles-per-hour", DisplayName = "Miles per Hour", Category = "Speed",
            Aliases = ["mph", "mile per hour"],
            ToBase = v => v * 0.44704,
            FromBase = v => v / 0.44704
        },
        new()
        {
            Key = "knots", DisplayName = "Knots", Category = "Speed",
            Aliases = ["knot", "kt", "kn"],
            ToBase = v => v * 0.514444,
            FromBase = v => v / 0.514444
        },
        new()
        {
            Key = "feet-per-second", DisplayName = "Feet per Second", Category = "Speed",
            Aliases = ["ft/s", "fps", "foot per second"],
            ToBase = v => v * 0.3048,
            FromBase = v => v / 0.3048
        },
    ];

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// All registered units, indexed by their canonical key (lowercase).
    /// </summary>
    public static IReadOnlyDictionary<string, UnitDefinition> AllUnits { get; } =
        BuildIndex(LengthUnits, TemperatureUnits, WeightUnits, VolumeUnits, SpeedUnits);

    /// <summary>
    /// Alias index: maps every alias (and the key itself) to its UnitDefinition.
    /// Lookups are case-insensitive.
    /// </summary>
    public static IReadOnlyDictionary<string, UnitDefinition> AliasIndex { get; } =
        BuildAliasIndex(AllUnits.Values);

    /// <summary>All distinct category names.</summary>
    public static IReadOnlyList<string> Categories { get; } =
        AllUnits.Values.Select(u => u.Category).Distinct().Order().ToList();

    private static IReadOnlyDictionary<string, UnitDefinition> BuildIndex(
        params IEnumerable<UnitDefinition>[] groups)
    {
        var dict = new Dictionary<string, UnitDefinition>(StringComparer.OrdinalIgnoreCase);
        foreach (var unit in groups.SelectMany(g => g))
            dict[unit.Key] = unit;
        return dict;
    }

    private static IReadOnlyDictionary<string, UnitDefinition> BuildAliasIndex(
        IEnumerable<UnitDefinition> units)
    {
        var dict = new Dictionary<string, UnitDefinition>(StringComparer.OrdinalIgnoreCase);
        foreach (var unit in units)
        {
            dict[unit.Key] = unit;
            foreach (var alias in unit.Aliases)
                dict.TryAdd(alias, unit); // first definition wins if aliases clash
        }
        return dict;
    }
}
