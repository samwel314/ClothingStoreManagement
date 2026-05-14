using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClothingStoreManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class addClumnToShiftModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalAdjustments",
                table: "Shifts",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalExpenses",
                table: "Shifts",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAdjustments",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "TotalExpenses",
                table: "Shifts");
        }
    }
}
