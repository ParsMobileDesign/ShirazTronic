using Microsoft.EntityFrameworkCore.Migrations;

namespace ShirazTronic.Data.Migrations
{
    public partial class addSomeFiledToProductSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Product",
                newName: "UnitPrice");

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Product",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BuyingPrice",
                table: "Product",
                type: "decimal(7,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PartNumber",
                table: "Product",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "BuyingPrice",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "PartNumber",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "Product",
                newName: "Price");
        }
    }
}
