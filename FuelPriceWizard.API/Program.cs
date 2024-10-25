using FuelPriceWizard.API.DTOs.Mapping;
using FuelPriceWizard.BusinessLogic;
using FuelPriceWizard.DataAccess;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddFuelPriceWizardBusinessLogic();
builder.Services.AddFuelPriceWizardDataAccess(builder.Configuration.GetConnectionString("FuelPriceWizard_Dev")!);

builder.Services.AddAutoMapper(
    typeof(AddressDtoMappingProfile),
    typeof(FuelTypeDtoMappingProfile),
    typeof(GasStationDtoMappingProfile),
    typeof(OpeningHoursDtoMappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
