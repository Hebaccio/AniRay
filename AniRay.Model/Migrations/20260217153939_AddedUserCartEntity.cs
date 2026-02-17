using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AniRay.Model.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserCartEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserCartId",
                table: "BluRayCarts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserCarts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CartNotes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCarts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCarts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BluRayCarts_UserCartId",
                table: "BluRayCarts",
                column: "UserCartId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCarts_UserId",
                table: "UserCarts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BluRayCarts_UserCarts_UserCartId",
                table: "BluRayCarts",
                column: "UserCartId",
                principalTable: "UserCarts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BluRayCarts_UserCarts_UserCartId",
                table: "BluRayCarts");

            migrationBuilder.DropTable(
                name: "UserCarts");

            migrationBuilder.DropIndex(
                name: "IX_BluRayCarts_UserCartId",
                table: "BluRayCarts");

            migrationBuilder.DropColumn(
                name: "UserCartId",
                table: "BluRayCarts");
        }
    }
}
