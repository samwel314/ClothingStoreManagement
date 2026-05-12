using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClothingStoreManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateShiftTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "FinalCashInDrawer",
                table: "Shifts",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Difference",
                table: "Shifts",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalReturns",
                table: "Shifts",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalSalesCash",
                table: "Shifts",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalSalesNonCash",
                table: "Shifts",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Difference",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "TotalReturns",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "TotalSalesCash",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "TotalSalesNonCash",
                table: "Shifts");

            migrationBuilder.AlterColumn<double>(
                name: "FinalCashInDrawer",
                table: "Shifts",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "REAL");
        }
    }
}
