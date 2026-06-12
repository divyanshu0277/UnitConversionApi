using FluentAssertions;
using UnitConversionApi.Models;
using UnitConversionApi.Services;

namespace UnitConversionApi.Tests.Services;

public sealed class ConversionServiceTests
{
    private readonly ConversionService _sut = new();

    // ── Length ────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1, "meters", "feet", 3.280839895)]
    [InlineData(1, "kilometers", "miles", 0.6213711922)]
    [InlineData(100, "centimeters", "meters", 1)]
    [InlineData(1, "miles", "kilometers", 1.609344)]
    [InlineData(1, "yards", "meters", 0.9144)]
    [InlineData(12, "inches", "feet", 1)]
    public void Convert_Length_ReturnsCorrectResult(
        double input, string from, string to, double expected)
    {
        var result = _sut.Convert(new ConversionRequest { Value = input, FromUnit = from, ToUnit = to });

        result.IsSuccess.Should().BeTrue();
        result.AsSuccess().OutputValue.Should().BeApproximately(expected, precision: 6);
        result.AsSuccess().Category.Should().Be("Length");
    }

    // ── Temperature ───────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0, "celsius", "fahrenheit", 32)]
    [InlineData(100, "celsius", "fahrenheit", 212)]
    [InlineData(0, "celsius", "kelvin", 273.15)]
    [InlineData(32, "fahrenheit", "celsius", 0)]
    [InlineData(212, "fahrenheit", "celsius", 100)]
    [InlineData(273.15, "kelvin", "celsius", 0)]
    public void Convert_Temperature_ReturnsCorrectResult(
        double input, string from, string to, double expected)
    {
        var result = _sut.Convert(new ConversionRequest { Value = input, FromUnit = from, ToUnit = to });

        result.IsSuccess.Should().BeTrue();
        result.AsSuccess().OutputValue.Should().BeApproximately(expected, precision: 6);
        result.AsSuccess().Category.Should().Be("Temperature");
    }

    // ── Weight ────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1, "kilograms", "pounds", 2.2046226218)]
    [InlineData(1, "pounds", "kilograms", 0.45359237)]
    [InlineData(1000, "grams", "kilograms", 1)]
    [InlineData(1, "metric-tons", "kilograms", 1000)]
    public void Convert_Weight_ReturnsCorrectResult(
        double input, string from, string to, double expected)
    {
        var result = _sut.Convert(new ConversionRequest { Value = input, FromUnit = from, ToUnit = to });

        result.IsSuccess.Should().BeTrue();
        result.AsSuccess().OutputValue.Should().BeApproximately(expected, precision: 6);
        result.AsSuccess().Category.Should().Be("Weight");
    }

    // ── Volume ────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1, "liters", "milliliters", 1000)]
    [InlineData(1, "gallons-us", "liters", 3.785411784)]
    [InlineData(1, "cubic-meters", "liters", 1000)]
    public void Convert_Volume_ReturnsCorrectResult(
        double input, string from, string to, double expected)
    {
        var result = _sut.Convert(new ConversionRequest { Value = input, FromUnit = from, ToUnit = to });

        result.IsSuccess.Should().BeTrue();
        result.AsSuccess().OutputValue.Should().BeApproximately(expected, precision: 6);
    }

    // ── Speed ─────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1, "meters-per-second", "kilometers-per-hour", 3.6)]
    [InlineData(100, "kilometers-per-hour", "miles-per-hour", 62.1371192237)]
    public void Convert_Speed_ReturnsCorrectResult(
        double input, string from, string to, double expected)
    {
        var result = _sut.Convert(new ConversionRequest { Value = input, FromUnit = from, ToUnit = to });

        result.IsSuccess.Should().BeTrue();
        result.AsSuccess().OutputValue.Should().BeApproximately(expected, precision: 6);
    }

    // ── Alias resolution ──────────────────────────────────────────────────────

    [Theory]
    [InlineData("kg", "lb")]
    [InlineData("m", "ft")]
    [InlineData("°c", "°f")]
    [InlineData("km/h", "mph")]
    [InlineData("KG", "LB")]   // case-insensitive
    public void Convert_AcceptsAliasesAndIsCaseInsensitive(string from, string to)
    {
        var result = _sut.Convert(new ConversionRequest { Value = 1, FromUnit = from, ToUnit = to });

        result.IsSuccess.Should().BeTrue();
    }

    // ── Same-unit identity ────────────────────────────────────────────────────

    [Theory]
    [InlineData(42.5, "meters", "meters")]
    [InlineData(100, "celsius", "celsius")]
    [InlineData(0, "kilograms", "kilograms")]
    public void Convert_SameUnit_ReturnsIdenticalValue(double value, string from, string to)
    {
        var result = _sut.Convert(new ConversionRequest { Value = value, FromUnit = from, ToUnit = to });

        result.IsSuccess.Should().BeTrue();
        result.AsSuccess().OutputValue.Should().BeApproximately(value, precision: 10);
    }

    // ── Error cases ───────────────────────────────────────────────────────────

    [Fact]
    public void Convert_UnknownFromUnit_ReturnsError()
    {
        var result = _sut.Convert(new ConversionRequest
        {
            Value = 1, FromUnit = "parsecs", ToUnit = "meters"
        });

        result.IsSuccess.Should().BeFalse();
        result.AsError().Message.Should().Contain("parsecs");
    }

    [Fact]
    public void Convert_UnknownToUnit_ReturnsError()
    {
        var result = _sut.Convert(new ConversionRequest
        {
            Value = 1, FromUnit = "meters", ToUnit = "furlongs-per-fortnight"
        });

        result.IsSuccess.Should().BeFalse();
        result.AsError().Message.Should().Contain("furlongs-per-fortnight");
    }

    [Fact]
    public void Convert_CrossCategoryConversion_ReturnsError()
    {
        var result = _sut.Convert(new ConversionRequest
        {
            Value = 1, FromUnit = "meters", ToUnit = "kilograms"
        });

        result.IsSuccess.Should().BeFalse();
        result.AsError().Message.Should().Contain("Length");
        result.AsError().Message.Should().Contain("Weight");
    }

    [Theory]
    [InlineData("", "meters")]
    [InlineData("meters", "")]
    public void Convert_EmptyUnitStrings_ReturnsError(string from, string to)
    {
        var result = _sut.Convert(new ConversionRequest { Value = 1, FromUnit = from, ToUnit = to });

        result.IsSuccess.Should().BeFalse();
    }

    // ── GetSupportedUnits ─────────────────────────────────────────────────────

    [Fact]
    public void GetSupportedUnits_ReturnsAllUnitsWhenNoCategoryFilter()
    {
        var units = _sut.GetSupportedUnits();

        units.Should().NotBeEmpty();
        units.Select(u => u.Category).Distinct().Should().HaveCountGreaterThan(1);
    }

    [Theory]
    [InlineData("Length")]
    [InlineData("Temperature")]
    [InlineData("Weight")]
    [InlineData("Volume")]
    [InlineData("Speed")]
    public void GetSupportedUnits_FiltersByCategory(string category)
    {
        var units = _sut.GetSupportedUnits(category);

        units.Should().NotBeEmpty();
        units.Should().AllSatisfy(u => u.Category.Should().Be(category));
    }

    [Fact]
    public void GetSupportedUnits_UnknownCategoryReturnsEmpty()
    {
        var units = _sut.GetSupportedUnits("Madeup");

        units.Should().BeEmpty();
    }

    // ── GetCategories ─────────────────────────────────────────────────────────

    [Fact]
    public void GetCategories_ReturnsExpectedCategories()
    {
        var categories = _sut.GetCategories();

        categories.Should().Contain(["Length", "Temperature", "Weight", "Volume", "Speed"]);
    }
}
