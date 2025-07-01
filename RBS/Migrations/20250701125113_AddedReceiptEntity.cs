using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RBS.Migrations
{
    /// <inheritdoc />
    public partial class AddedReceiptEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReceiptId",
                table: "Tables",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReceiptId",
                table: "Spaces",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReceiptId",
                table: "Chairs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerDetailsId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receipts_AspNetUsers_CustomerDetailsId",
                        column: x => x.CustomerDetailsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tables_ReceiptId",
                table: "Tables",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_Spaces_ReceiptId",
                table: "Spaces",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_Chairs_ReceiptId",
                table: "Chairs",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_CustomerDetailsId",
                table: "Receipts",
                column: "CustomerDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chairs_Receipts_ReceiptId",
                table: "Chairs",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Spaces_Receipts_ReceiptId",
                table: "Spaces",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_Receipts_ReceiptId",
                table: "Tables",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chairs_Receipts_ReceiptId",
                table: "Chairs");

            migrationBuilder.DropForeignKey(
                name: "FK_Spaces_Receipts_ReceiptId",
                table: "Spaces");

            migrationBuilder.DropForeignKey(
                name: "FK_Tables_Receipts_ReceiptId",
                table: "Tables");

            migrationBuilder.DropTable(
                name: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Tables_ReceiptId",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_Spaces_ReceiptId",
                table: "Spaces");

            migrationBuilder.DropIndex(
                name: "IX_Chairs_ReceiptId",
                table: "Chairs");

            migrationBuilder.DropColumn(
                name: "ReceiptId",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "ReceiptId",
                table: "Spaces");

            migrationBuilder.DropColumn(
                name: "ReceiptId",
                table: "Chairs");
        }
    }
}
