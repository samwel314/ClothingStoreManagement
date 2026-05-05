using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClothingStoreManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateInvocieModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmountWithDiscount",
                table: "Invoices",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmountWithDiscount",
                table: "Invoices");
        }
    }
}
