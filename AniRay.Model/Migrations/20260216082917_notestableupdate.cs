using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AniRay.Model.Migrations
{
    /// <inheritdoc />
    public partial class notestableupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserCity",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserCountry",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserCity",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UserCountry",
                table: "Orders");
        }
    }
}
