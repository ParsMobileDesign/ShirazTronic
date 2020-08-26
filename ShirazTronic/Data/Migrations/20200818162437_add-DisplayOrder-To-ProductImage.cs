using Microsoft.EntityFrameworkCore.Migrations;

namespace ShirazTronic.Data.Migrations
{
    public partial class addDisplayOrderToProductImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "DisplayOrder",
                table: "ProductImage",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "ProductImage");
        }
    }
}
