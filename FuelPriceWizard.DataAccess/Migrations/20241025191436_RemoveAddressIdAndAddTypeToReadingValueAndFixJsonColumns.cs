using FuelPriceWizard.DataAccess.Constants;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuelPriceWizard.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAddressIdAndAddTypeToReadingValueAndFixJsonColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressId",
                table: MigrationConstants.GAS_STATIONS);

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "PriceReadings",
                type: "decimal(4,3)",
                precision: 4,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(3,2)",
                oldPrecision: 3);

            migrationBuilder.AlterColumn<string>(
                name: "OpeningHours",
                table: MigrationConstants.GAS_STATIONS,
                type: MigrationConstants.N_VARCHAR_MAX,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: MigrationConstants.N_VARCHAR_MAX,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "PriceReadings",
                type: "decimal(3,2)",
                precision: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(4,3)",
                oldPrecision: 4,
                oldScale: 3);

            migrationBuilder.AlterColumn<string>(
                name: "OpeningHours",
                table: MigrationConstants.GAS_STATIONS,
                type: MigrationConstants.N_VARCHAR_MAX,
                nullable: true,
                oldClrType: typeof(string),
                oldType: MigrationConstants.N_VARCHAR_MAX);

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: MigrationConstants.GAS_STATIONS,
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
