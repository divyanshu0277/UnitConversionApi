# Unit Conversion API

A RESTful ASP.NET Core Web API for converting numerical values between units of measurement.

## Features

- **5 conversion categories** out of the box: Length, Temperature, Weight, Volume, Speed
- **40+ units** with common aliases (e.g. `kg`, `kilogram`, `kilograms` all resolve to Kilograms)
- Case-insensitive unit matching
- Interactive Swagger UI available in development
- Full unit and integration test coverage
- Dark and light mode

## Project Structure

```
UnitConversionApi/
├── src/
│   └── UnitConversionApi/
│       ├── Configuration/
│       │   └── UnitRegistry.cs      # All unit definitions & alias index
│       ├── Controllers/
│       │   └── ConversionController.cs
│       ├── Models/
│       │   └── ConversionModels.cs  # Request/response DTOs
│       ├── Services/
│       │   ├── IConversionService.cs
│       │   ├── ConversionService.cs
│       │   └── OneOf.cs             # Lightweight discriminated union
│       └── Program.cs
└── tests/
    └── UnitConversionApi.Tests/
        ├── Controllers/
        │   └── ConversionControllerTests.cs  # Integration tests
        └── Services/
            └── ConversionServiceTests.cs     # Unit tests
```

## Web UI

A lightweight static web UI is included at `wwwroot/index.html` and served directly by the API at the root URL (`/`).

- Convert between units using a simple form (category pills, from/to selectors, swap button)
- Displays the conversion formula used (e.g. `100°C × 9/5 + 32 = 212°F`)
- Copy-to-clipboard button for quick reuse of results
- Dark mode toggle (preference persisted in `localStorage`)
- Keeps a small history of recent conversions

## Running Locally

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (or later)

### Steps

```bash
# Clone the repository
git clone <repository-url>
cd UnitConversionApi

# Run the API
dotnet run --project src/UnitConversionApi

# The API starts on https://localhost:5001 (or http://localhost:5000)
# Swagger UI is available at https://localhost:5001 in development
```

### Running Tests

```bash
dotnet test
```

## API Reference

### `POST /api/v1/convert`

Converts a value from one unit to another.

**Request body:**

```json
{
  "value": 100,
  "fromUnit": "celsius",
  "toUnit": "fahrenheit"
}
```

**Success response (200):**

```json
{
  "inputValue": 100,
  "fromUnit": "Celsius",
  "toUnit": "Fahrenheit",
  "outputValue": 212.0,
  "category": "Temperature"
}
```

**Error response (400):**

```json
{
  "message": "Cannot convert between 'Meters' (Length) and 'Kilograms' (Weight).",
  "detail": "Units must belong to the same category."
}
```

### `GET /api/v1/units`

Returns all supported units. Optionally filter by category:

```
GET /api/v1/units?category=Length
```

### `GET /api/v1/categories`

Returns all supported category names.

---

## Supported Units

| Category    | Units (key)                                                                          |
|-------------|--------------------------------------------------------------------------------------|
| Length      | meters, kilometers, centimeters, millimeters, miles, yards, feet, inches, nautical-miles |
| Temperature | celsius, fahrenheit, kelvin, rankine                                                 |
| Weight      | kilograms, grams, milligrams, pounds, ounces, metric-tons, short-tons, stone         |
| Volume      | liters, milliliters, cubic-meters, gallons-us, gallons-uk, fluid-ounces, cups, pints, quarts |
| Speed       | meters-per-second, kilometers-per-hour, miles-per-hour, knots, feet-per-second       |

All units accept common aliases (e.g. `kg`, `kilogram`, `kgs`). Use `GET /api/v1/units` to see all aliases.

---

## Design Decisions

### Base-unit conversion strategy

Rather than defining N² pairwise conversion functions, each unit stores only two functions:
- `ToBase` — converts from the unit to the category's base unit (e.g. meters for Length, Celsius for Temperature)
- `FromBase` — converts from the base unit to the unit

Every conversion is then a two-hop: `input → base → output`. This makes adding new units trivial and keeps the unit registry declarative.

### Alias-based unit resolution

Each `UnitDefinition` carries a list of aliases. At startup, `UnitRegistry` builds a flat lookup dictionary across all aliases. This means `"kg"`, `"kilogram"`, `"kilograms"`, and `"kgs"` all resolve to the same definition, with no special-casing in the service layer.

### Discriminated union result type (`OneOf<TSuccess, TError>`)

`ConversionService.Convert` returns `OneOf<ConversionResult, ConversionError>` instead of throwing exceptions or returning nullable types. This makes the success/failure paths explicit at the call site and avoids exception-driven control flow for predictable business errors. A minimal inline implementation is included to avoid an external dependency for a single use-case.

### Hardcoded registry, designed for future extensibility

Units are hardcoded per the challenge spec. The `UnitRegistry` is intentionally structured so that switching to a database-backed store in the future only requires injecting an `IUnitRepository` into `ConversionService` — the service logic and controller would not change.

### Rounding

Output values are rounded to 10 decimal places to avoid floating-point noise in display, while retaining enough precision for scientific use.
