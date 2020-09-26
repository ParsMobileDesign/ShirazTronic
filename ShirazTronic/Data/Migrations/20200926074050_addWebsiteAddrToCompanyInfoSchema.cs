using Microsoft.EntityFrameworkCore.Migrations;

namespace ShirazTronic.Data.Migrations
{
    public partial class addWebsiteAddrToCompanyInfoSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Manufacturer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "WebsiteAddr",
                table: "CompanyInfo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebsiteAddr",
                table: "CompanyInfo");

            migrationBuilder.AlterColumn<short>(
                name: "Id",
                table: "Manufacturer",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }
    }
}
