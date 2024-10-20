using FuelPriceWizard.DataAccess.Constants;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuelPriceWizard.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation(MigrationConstants.SQL_SERVER_IDENTITY, "1, 1"),
                    Name = table.Column<string>(type: MigrationConstants.N_VARCHAR_MAX, nullable: false),
                    Abbreviation = table.Column<string>(type: MigrationConstants.N_VARCHAR_MAX, nullable: false),
                    Symbol = table.Column<string>(type: MigrationConstants.N_VARCHAR_MAX, nullable: false),
                    PriceReadingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: MigrationConstants.FUEL_TYPES,
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation(MigrationConstants.SQL_SERVER_IDENTITY, "1, 1"),
                    DisplayValue = table.Column<string>(type: MigrationConstants.N_VARCHAR_MAX, nullable: false),
                    Abbreviation = table.Column<string>(type: MigrationConstants.N_VARCHAR_MAX, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelTypeId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: MigrationConstants.GAS_STATIONS,
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation(MigrationConstants.SQL_SERVER_IDENTITY, "1, 1"),
                    Designation = table.Column<string>(type: MigrationConstants.N_VARCHAR_MAX, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AddressId = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: MigrationConstants.N_VARCHAR_MAX, nullable: true),
                    OpeningHours = table.Column<string>(type: MigrationConstants.N_VARCHAR_MAX, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GasStationId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FuelTypeGasStation",
                columns: table => new
                {
                    FuelTypesId = table.Column<int>(type: "int", nullable: false),
                    GasStationsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelTypeGasStation", x => new { x.FuelTypesId, x.GasStationsId });
                    table.ForeignKey(
                        name: "FK_FuelTypeGasStation_FuelTypes_FuelTypesId",
                        column: x => x.FuelTypesId,
                        principalTable: MigrationConstants.FUEL_TYPES,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FuelTypeGasStation_GasStations_GasStationsId",
                        column: x => x.GasStationsId,
                        principalTable: MigrationConstants.GAS_STATIONS,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: MigrationConstants.PRICE_READINGS,
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation(MigrationConstants.SQL_SERVER_IDENTITY, "1, 1"),
                    Value = table.Column<decimal>(type: "decimal(3,2)", precision: 3, nullable: false),
                    FetchedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    FuelTypeId = table.Column<int>(type: "int", nullable: false),
                    GasStationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceReadingId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceReadings_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PriceReadings_FuelTypes_FuelTypeId",
                        column: x => x.FuelTypeId,
                        principalTable: MigrationConstants.FUEL_TYPES,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PriceReadings_GasStations_GasStationId",
                        column: x => x.GasStationId,
                        principalTable: MigrationConstants.GAS_STATIONS,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FuelTypeGasStation_GasStationsId",
                table: "FuelTypeGasStation",
                column: "GasStationsId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceReadings_CurrencyId",
                table: MigrationConstants.PRICE_READINGS,
                column: "CurrencyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceReadings_FuelTypeId",
                table: MigrationConstants.PRICE_READINGS,
                column: "FuelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceReadings_GasStationId",
                table: MigrationConstants.PRICE_READINGS,
                column: "GasStationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FuelTypeGasStation");

            migrationBuilder.DropTable(
                name: MigrationConstants.PRICE_READINGS);

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: MigrationConstants.FUEL_TYPES);

            migrationBuilder.DropTable(
                name: MigrationConstants.GAS_STATIONS);
        }
    }
}
