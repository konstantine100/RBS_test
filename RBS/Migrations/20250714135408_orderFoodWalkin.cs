using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RBS.Migrations
{
    /// <inheritdoc />
    public partial class orderFoodWalkin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderedFoods_Bookings_BookingId",
                table: "OrderedFoods");

            migrationBuilder.AlterColumn<int>(
                name: "BookingId",
                table: "OrderedFoods",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "WalkInId",
                table: "OrderedFoods",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderedFoods_WalkInId",
                table: "OrderedFoods",
                column: "WalkInId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedFoods_Bookings_BookingId",
                table: "OrderedFoods",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedFoods_WalkIns_WalkInId",
                table: "OrderedFoods",
                column: "WalkInId",
                principalTable: "WalkIns",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderedFoods_Bookings_BookingId",
                table: "OrderedFoods");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderedFoods_WalkIns_WalkInId",
                table: "OrderedFoods");

            migrationBuilder.DropIndex(
                name: "IX_OrderedFoods_WalkInId",
                table: "OrderedFoods");

            migrationBuilder.DropColumn(
                name: "WalkInId",
                table: "OrderedFoods");

            migrationBuilder.AlterColumn<int>(
                name: "BookingId",
                table: "OrderedFoods",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedFoods_Bookings_BookingId",
                table: "OrderedFoods",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
