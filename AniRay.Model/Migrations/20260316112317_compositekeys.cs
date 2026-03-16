using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AniRay.Model.Migrations
{
    /// <inheritdoc />
    public partial class compositekeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFavorites",
                table: "UserFavorites");

            migrationBuilder.DropIndex(
                name: "IX_UserFavorites_UserId",
                table: "UserFavorites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderBluRays",
                table: "OrderBluRays");

            migrationBuilder.DropIndex(
                name: "IX_OrderBluRays_OrderId",
                table: "OrderBluRays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieGenres",
                table: "MovieGenres");

            migrationBuilder.DropIndex(
                name: "IX_MovieGenres_MovieId",
                table: "MovieGenres");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BluRayCarts",
                table: "BluRayCarts");

            migrationBuilder.DropIndex(
                name: "IX_BluRayCarts_UserCartId",
                table: "BluRayCarts");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserFavorites");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OrderBluRays");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MovieGenres");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "BluRayCarts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFavorites",
                table: "UserFavorites",
                columns: new[] { "UserId", "MovieId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderBluRays",
                table: "OrderBluRays",
                columns: new[] { "OrderId", "BluRayId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieGenres",
                table: "MovieGenres",
                columns: new[] { "MovieId", "GenreId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BluRayCarts",
                table: "BluRayCarts",
                columns: new[] { "UserCartId", "BluRayId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFavorites",
                table: "UserFavorites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderBluRays",
                table: "OrderBluRays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieGenres",
                table: "MovieGenres");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BluRayCarts",
                table: "BluRayCarts");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserFavorites",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "OrderBluRays",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "MovieGenres",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "BluRayCarts",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFavorites",
                table: "UserFavorites",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderBluRays",
                table: "OrderBluRays",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieGenres",
                table: "MovieGenres",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BluRayCarts",
                table: "BluRayCarts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_UserId",
                table: "UserFavorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBluRays_OrderId",
                table: "OrderBluRays",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenres_MovieId",
                table: "MovieGenres",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_BluRayCarts_UserCartId",
                table: "BluRayCarts",
                column: "UserCartId");
        }
    }
}
