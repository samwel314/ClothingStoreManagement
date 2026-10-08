using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClothingStoreManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class addSuppliyersModeld : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "TreasuryTransactions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    AdditionalPhone = table.Column<string>(type: "TEXT", nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupplierPurchases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SupplierId = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalAmount = table.Column<double>(type: "REAL", nullable: false),
                    PaidAmount = table.Column<double>(type: "REAL", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierPurchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierPurchases_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TreasuryTransactions_SupplierId",
                table: "TreasuryTransactions",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPurchases_SupplierId",
                table: "SupplierPurchases",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_TreasuryTransactions_Suppliers_SupplierId",
                table: "TreasuryTransactions",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TreasuryTransactions_Suppliers_SupplierId",
                table: "TreasuryTransactions");

            migrationBuilder.DropTable(
                name: "SupplierPurchases");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_TreasuryTransactions_SupplierId",
                table: "TreasuryTransactions");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "TreasuryTransactions");
        }
    }
}
