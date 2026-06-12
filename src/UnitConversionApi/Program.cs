using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnitConversionApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddScoped<IConversionService, ConversionService>();

// ── OpenAPI / Swagger ─────────────────────────────────────────────────────────

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Unit Conversion API",
        Version = "v1",
        Description = """
            A RESTful API for converting numerical values between units of measurement.
            
            Supported categories: **Length**, **Temperature**, **Weight**, **Volume**, **Speed**.
            
            Unit names and aliases are matched case-insensitively.
            Use `GET /api/v1/units` to discover all supported unit keys and aliases.
            """
    });

    // Include XML doc comments in Swagger UI
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

// ── Middleware pipeline ───────────────────────────────────────────────────────

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Unit Conversion API v1");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

app.Run();

// Make Program accessible for integration tests (WebApplicationFactory)
public partial class Program { }
