using Microsoft.EntityFrameworkCore.Migrations;

namespace ShirazTronic.Data.Migrations
{
    public partial class changeStatustoOrderStatusInMemOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "MemOrder");

            migrationBuilder.AddColumn<byte>(
                name: "OrderStatus",
                table: "MemOrder",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "MemOrder");

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "MemOrder",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
