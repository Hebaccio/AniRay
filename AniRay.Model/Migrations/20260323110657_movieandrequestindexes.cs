using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AniRay.Model.Migrations
{
    /// <inheritdoc />
    public partial class movieandrequestindexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Requests",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "ReadByStaff",
                table: "Requests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReadByUser",
                table: "Requests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Response",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Movies",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Studio",
                table: "Movies",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Director",
                table: "Movies",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_DateTime",
                table: "Requests",
                column: "DateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Title",
                table: "Requests",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Director",
                table: "Movies",
                column: "Director");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Favorites",
                table: "Movies",
                column: "Favorites");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_IsDeleted",
                table: "Movies",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_IsDeleted_Favorites",
                table: "Movies",
                columns: new[] { "IsDeleted", "Favorites" });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_IsDeleted_ReleaseDate",
                table: "Movies",
                columns: new[] { "IsDeleted", "ReleaseDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_ReleaseDate",
                table: "Movies",
                column: "ReleaseDate");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Studio",
                table: "Movies",
                column: "Studio");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Title",
                table: "Movies",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenres_GenreId_MovieId",
                table: "MovieGenres",
                columns: new[] { "GenreId", "MovieId" });

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenres_MovieId",
                table: "MovieGenres",
                column: "MovieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Requests_DateTime",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_Title",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Director",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Favorites",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_IsDeleted",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_IsDeleted_Favorites",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_IsDeleted_ReleaseDate",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_ReleaseDate",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Studio",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Title",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_MovieGenres_GenreId_MovieId",
                table: "MovieGenres");

            migrationBuilder.DropIndex(
                name: "IX_MovieGenres_MovieId",
                table: "MovieGenres");

            migrationBuilder.DropColumn(
                name: "ReadByStaff",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "ReadByUser",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Response",
                table: "Requests");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Studio",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Director",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
