using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AniRay.Model.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BluRayCarts_UserCarts_UserCartId",
                table: "BluRayCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_BluRayCarts_Users_UserId",
                table: "BluRayCarts");

            migrationBuilder.DropIndex(
                name: "IX_BluRayCarts_UserId",
                table: "BluRayCarts");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BluRayCarts",
                newName: "CartId");

            migrationBuilder.AddColumn<double>(
                name: "FullCartPrice",
                table: "UserCarts",
                type: "float(18)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<int>(
                name: "UserCartId",
                table: "BluRayCarts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BluRayCarts_UserCarts_UserCartId",
                table: "BluRayCarts",
                column: "UserCartId",
                principalTable: "UserCarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BluRayCarts_UserCarts_UserCartId",
                table: "BluRayCarts");

            migrationBuilder.DropColumn(
                name: "FullCartPrice",
                table: "UserCarts");

            migrationBuilder.RenameColumn(
                name: "CartId",
                table: "BluRayCarts",
                newName: "UserId");

            migrationBuilder.AlterColumn<int>(
                name: "UserCartId",
                table: "BluRayCarts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_BluRayCarts_UserId",
                table: "BluRayCarts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BluRayCarts_UserCarts_UserCartId",
                table: "BluRayCarts",
                column: "UserCartId",
                principalTable: "UserCarts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BluRayCarts_Users_UserId",
                table: "BluRayCarts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
