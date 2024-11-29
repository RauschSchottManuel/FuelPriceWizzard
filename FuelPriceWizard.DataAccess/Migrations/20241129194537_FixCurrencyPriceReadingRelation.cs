using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuelPriceWizard.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixCurrencyPriceReadingRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PriceReadings_CurrencyId",
                table: "PriceReadings");

            migrationBuilder.DropColumn(
                name: "PriceReadingId",
                table: "Currencies");

            migrationBuilder.CreateIndex(
                name: "IX_PriceReadings_CurrencyId",
                table: "PriceReadings",
                column: "CurrencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PriceReadings_CurrencyId",
                table: "PriceReadings");

            migrationBuilder.AddColumn<int>(
                name: "PriceReadingId",
                table: "Currencies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PriceReadings_CurrencyId",
                table: "PriceReadings",
                column: "CurrencyId",
                unique: true);
        }
    }
}
