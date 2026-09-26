using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClothingStoreManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class seedDataTo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PaymentSources",
                columns: new[] { "Id", "IsActive", "IsCashSource", "Name" },
                values: new object[] { 1, true, true, "كاش (نقدي)" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$0.EXpw9/TA95xTDBf5rqWOa7eg2cqZC8ENIXtGqGRgAFG15UbGlJa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PaymentSources",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$evS/J.Lp6vL8vL8vL8vL8ueXGvS/J.Lp6vL8vL8vL8vL8ueXG");
        }
    }
}
