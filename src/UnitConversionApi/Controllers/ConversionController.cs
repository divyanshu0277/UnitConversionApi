using Microsoft.AspNetCore.Mvc;
using UnitConversionApi.Models;
using UnitConversionApi.Services;

namespace UnitConversionApi.Controllers;

/// <summary>
/// Exposes endpoints for converting values between units of measurement
/// and for discovering which units the API supports.
/// </summary>
[ApiController]
[Route("api/v1")]
[Produces("application/json")]
public sealed class ConversionController(IConversionService conversionService) : ControllerBase
{
    /// <summary>
    /// Converts a numeric value from one unit to another.
    /// </summary>
    /// <remarks>
    /// Units are matched case-insensitively and common aliases are accepted.
    /// For example, "kg", "kilogram", and "kilograms" all resolve to Kilograms.
    /// Use <c>GET /api/v1/units</c> to discover all supported unit keys and aliases.
    ///
    /// Example request:
    ///
    ///     POST /api/v1/convert
    ///     {
    ///       "value": 100,
    ///       "fromUnit": "celsius",
    ///       "toUnit": "fahrenheit"
    ///     }
    /// </remarks>
    /// <param name="request">The conversion parameters.</param>
    /// <returns>The converted value with metadata, or a descriptive error.</returns>
    /// <response code="200">Conversion successful.</response>
    /// <response code="400">Invalid request — unknown unit or incompatible categories.</response>
    [HttpPost("convert")]
    [ProducesResponseType(typeof(ConversionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ConversionError), StatusCodes.Status400BadRequest)]
    public IActionResult Convert([FromBody] ConversionRequest request)
    {
        var result = conversionService.Convert(request);

        return result.Match<IActionResult>(
            onSuccess: Ok,
            onError: error => BadRequest(error)
        );
    }

    /// <summary>
    /// Returns all supported units, optionally filtered by category.
    /// </summary>
    /// <param name="category">
    /// Optional category filter (e.g., "Length", "Temperature", "Weight", "Volume", "Speed").
    /// Case-insensitive. Returns all units when omitted.
    /// </param>
    /// <response code="200">List of supported units.</response>
    /// <response code="400">The specified category is not recognized.</response>
    [HttpGet("units")]
    [ProducesResponseType(typeof(IReadOnlyList<UnitInfo>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ConversionError), StatusCodes.Status400BadRequest)]
    public IActionResult GetUnits([FromQuery] string? category = null)
    {
        if (!string.IsNullOrWhiteSpace(category))
        {
            var validCategories = conversionService.GetCategories();
            if (!validCategories.Any(c => c.Equals(category, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest(new ConversionError
                {
                    Message = $"Unknown category: '{category}'.",
                    Detail = $"Supported categories: {string.Join(", ", validCategories)}."
                });
            }
        }

        return Ok(conversionService.GetSupportedUnits(category));
    }

    /// <summary>
    /// Returns all supported conversion categories.
    /// </summary>
    /// <response code="200">List of category names.</response>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public IActionResult GetCategories() => Ok(conversionService.GetCategories());
}
