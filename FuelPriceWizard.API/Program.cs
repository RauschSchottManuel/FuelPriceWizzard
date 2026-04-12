using FuelPriceWizard.API.DTOs.Mapping;
using FuelPriceWizard.BusinessLogic;
using FuelPriceWizard.DataAccess;
using FuelPriceWizard.DataAccess.Constants;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Controllers + ProblemDetails for standardised error responses
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FuelPriceWizard API",
        Version = "v1",
        Description = "API for querying gas stations and fuel price data."
    });

    // Include XML comments in Swagger UI
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

// Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Application services
builder.Services.AddFuelPriceWizardBusinessLogic();
builder.Services.AddFuelPriceWizardDataAccess(
    builder.Configuration.GetConnectionString(ConnectionStringConstants.FUEL_PRICE_WIZARD)!,
    builder.Environment.IsDevelopment());

// AutoMapper
builder.Services.AddAutoMapper(
    typeof(AddressDtoMappingProfile),
    typeof(CurrencyDtoMappingProfile),
    typeof(FuelTypeDtoMappingProfile),
    typeof(GasStationDtoMappingProfile),
    typeof(OpeningHoursDtoMappingProfile),
    typeof(PriceReadingDtoMappingProfile));

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<FuelPriceWizardDbContext>();

var app = builder.Build();

// Global exception handling — returns RFC 7807 ProblemDetails JSON for unhandled exceptions
app.UseExceptionHandler();
app.UseStatusCodePages();

// Swagger UI (development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FuelPriceWizard API v1"));
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");

await app.RunAsync();
