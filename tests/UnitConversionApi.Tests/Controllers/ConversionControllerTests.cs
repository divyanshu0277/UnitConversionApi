using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using UnitConversionApi.Models;

namespace UnitConversionApi.Tests.Controllers;

/// <summary>
/// Integration tests that spin up the real application pipeline
/// (routing, model binding, serialization) using WebApplicationFactory.
/// </summary>
public sealed class ConversionControllerTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private HttpClient Client => factory.CreateClient();

    // ── POST /api/v1/convert ──────────────────────────────────────────────────

    [Fact]
    public async Task Convert_ValidRequest_Returns200WithResult()
    {
        var request = new ConversionRequest
        {
            Value = 100,
            FromUnit = "celsius",
            ToUnit = "fahrenheit"
        };

        var response = await Client.PostAsJsonAsync("/api/v1/convert", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<ConversionResult>(JsonOptions);
        body.Should().NotBeNull();
        body!.OutputValue.Should().BeApproximately(212, precision: 0.001);
        body.Category.Should().Be("Temperature");
    }

    [Fact]
    public async Task Convert_UnknownUnit_Returns400WithError()
    {
        var request = new ConversionRequest
        {
            Value = 1,
            FromUnit = "lightyears",
            ToUnit = "meters"
        };

        var response = await Client.PostAsJsonAsync("/api/v1/convert", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await response.Content.ReadFromJsonAsync<ConversionError>(JsonOptions);
        body.Should().NotBeNull();
        body!.Message.Should().Contain("lightyears");
    }

    [Fact]
    public async Task Convert_CrossCategoryUnits_Returns400WithError()
    {
        var request = new ConversionRequest
        {
            Value = 1,
            FromUnit = "kilometers",
            ToUnit = "pounds"
        };

        var response = await Client.PostAsJsonAsync("/api/v1/convert", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ── GET /api/v1/units ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetUnits_NoFilter_Returns200WithAllUnits()
    {
        var response = await Client.GetAsync("/api/v1/units");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var units = await response.Content.ReadFromJsonAsync<List<UnitInfo>>(JsonOptions);
        units.Should().NotBeEmpty();
        units!.Select(u => u.Category).Distinct().Should().HaveCountGreaterThan(1);
    }

    [Fact]
    public async Task GetUnits_WithValidCategory_ReturnsFilteredUnits()
    {
        var response = await Client.GetAsync("/api/v1/units?category=Length");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var units = await response.Content.ReadFromJsonAsync<List<UnitInfo>>(JsonOptions);
        units.Should().NotBeEmpty();
        units!.Should().AllSatisfy(u => u.Category.Should().Be("Length"));
    }

    [Fact]
    public async Task GetUnits_WithInvalidCategory_Returns400()
    {
        var response = await Client.GetAsync("/api/v1/units?category=Nonsense");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ── GET /api/v1/categories ────────────────────────────────────────────────

    [Fact]
    public async Task GetCategories_Returns200WithExpectedCategories()
    {
        var response = await Client.GetAsync("/api/v1/categories");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var categories = await response.Content.ReadFromJsonAsync<List<string>>(JsonOptions);
        categories.Should().Contain(["Length", "Temperature", "Weight", "Volume", "Speed"]);
    }
}
