using Microsoft.EntityFrameworkCore.Migrations;

namespace Jeasthamdev.Cqrs.Manipulator.Migrations
{
    public partial class NullableCategoryId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_Category_CategoryId",
                table: "OrderLines");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "OrderLines",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_Category_CategoryId",
                table: "OrderLines",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_Category_CategoryId",
                table: "OrderLines");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "OrderLines",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_Category_CategoryId",
                table: "OrderLines",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
