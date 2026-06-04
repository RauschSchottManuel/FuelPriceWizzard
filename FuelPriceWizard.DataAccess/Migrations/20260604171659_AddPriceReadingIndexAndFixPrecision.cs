using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuelPriceWizard.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceReadingIndexAndFixPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PriceReadings_GasStationId",
                table: "PriceReadings");

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "PriceReadings",
                type: "decimal(8,4)",
                precision: 8,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(4,3)",
                oldPrecision: 4,
                oldScale: 3);

            migrationBuilder.CreateIndex(
                name: "IX_PriceReadings_GasStationId_FuelTypeId_FetchedAt",
                table: "PriceReadings",
                columns: new[] { "GasStationId", "FuelTypeId", "FetchedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PriceReadings_GasStationId_FuelTypeId_FetchedAt",
                table: "PriceReadings");

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "PriceReadings",
                type: "decimal(4,3)",
                precision: 4,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,4)",
                oldPrecision: 8,
                oldScale: 4);

            migrationBuilder.CreateIndex(
                name: "IX_PriceReadings_GasStationId",
                table: "PriceReadings",
                column: "GasStationId");
        }
    }
}
