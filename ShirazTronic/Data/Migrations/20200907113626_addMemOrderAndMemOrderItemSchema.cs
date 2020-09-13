using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShirazTronic.Data.Migrations
{
    public partial class addMemOrderAndMemOrderItemSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MemOrder",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    TransactionId = table.Column<int>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    PaymentStatus = table.Column<byte>(nullable: false),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemOrder_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemOrderItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemOrderId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Count = table.Column<int>(nullable: false),
                    Price = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemOrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemOrderItem_MemOrder_MemOrderId",
                        column: x => x.MemOrderId,
                        principalTable: "MemOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemOrderItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemOrder_UserId",
                table: "MemOrder",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MemOrderItem_MemOrderId",
                table: "MemOrderItem",
                column: "MemOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_MemOrderItem_ProductId",
                table: "MemOrderItem",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemOrderItem");

            migrationBuilder.DropTable(
                name: "MemOrder");
        }
    }
}
