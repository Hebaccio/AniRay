using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AniRay.Model.Migrations
{
    /// <inheritdoc />
    public partial class AddedAttemptsToTwoFA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Attempt",
                table: "twoWayAuths",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attempt",
                table: "twoWayAuths");
        }
    }
}
