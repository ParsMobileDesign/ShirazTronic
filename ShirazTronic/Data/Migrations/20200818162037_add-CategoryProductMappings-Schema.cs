using Microsoft.EntityFrameworkCore.Migrations;

namespace ShirazTronic.Data.Migrations
{
    public partial class addCategoryProductMappingsSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryProductMappings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatId = table.Column<int>(nullable: false),
                    SubCatId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryProductMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryProductMappings_Category_CatId",
                        column: x => x.CatId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_CategoryProductMappings_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_CategoryProductMappings_SubCategory_SubCatId",
                        column: x => x.SubCatId,
                        principalTable: "SubCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryProductMappings_CatId",
                table: "CategoryProductMappings",
                column: "CatId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryProductMappings_ProductId",
                table: "CategoryProductMappings",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryProductMappings_SubCatId",
                table: "CategoryProductMappings",
                column: "SubCatId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryProductMappings");
        }
    }
}
